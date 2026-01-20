// AI Chat Application JavaScript

// State management
const state = {
    messageCount: 0,
    totalTokens: 0,
    isProcessing: false
};

// DOM elements
const elements = {
    form: null,
    messageInput: null,
    sendButton: null,
    messagesContainer: null,
    streamToggle: null,
    clearButton: null,
    systemPrompt: null,
    temperature: null,
    maxTokens: null,
    messageCountDisplay: null,
    totalTokensDisplay: null
};

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    initializeElements();
    attachEventListeners();
});

// Initialize DOM element references
function initializeElements() {
    elements.form = document.getElementById('chatForm');
    elements.messageInput = document.getElementById('messageInput');
    elements.sendButton = document.getElementById('sendButton');
    elements.messagesContainer = document.getElementById('messagesContainer');
    elements.streamToggle = document.getElementById('streamToggle');
    elements.clearButton = document.getElementById('clearButton');
    elements.systemPrompt = document.getElementById('systemPrompt');
    elements.temperature = document.getElementById('temperature');
    elements.maxTokens = document.getElementById('maxTokens');
    elements.messageCountDisplay = document.getElementById('messageCount');
    elements.totalTokensDisplay = document.getElementById('totalTokens');
}

// Attach event listeners
function attachEventListeners() {
    elements.form.addEventListener('submit', handleSubmit);
    elements.clearButton.addEventListener('click', clearChat);

    // Auto-focus on message input
    elements.messageInput.focus();

    // Handle Enter key (Shift+Enter for new line)
    elements.messageInput.addEventListener('keydown', function(e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            elements.form.dispatchEvent(new Event('submit'));
        }
    });
}

// Handle form submission
async function handleSubmit(e) {
    e.preventDefault();

    if (state.isProcessing) {
        return;
    }

    const message = elements.messageInput.value.trim();
    if (!message) {
        return;
    }

    // Clear input and disable form
    elements.messageInput.value = '';
    elements.messageInput.style.height = 'auto';
    setProcessingState(true);

    // Remove welcome message if present
    removeWelcomeMessage();

    // Add user message to chat
    addMessage(message, 'user');

    // Update statistics
    state.messageCount++;
    updateStatistics();

    // Build request
    const request = {
        message: message,
        systemPrompt: elements.systemPrompt.value || null,
        temperature: parseFloat(elements.temperature.value),
        maxTokens: parseInt(elements.maxTokens.value),
        stream: elements.streamToggle.checked
    };

    try {
        if (request.stream) {
            await handleStreamingResponse(request);
        } else {
            await handleRegularResponse(request);
        }
    } catch (error) {
        console.error('Error:', error);
        showError('Failed to get response. Please try again.');
    } finally {
        setProcessingState(false);
    }
}

// Handle regular (non-streaming) response
async function handleRegularResponse(request) {
    // Show typing indicator
    const typingId = showTypingIndicator();

    try {
        const response = await fetch('/Chat/SendMessage', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();

        // Remove typing indicator
        removeTypingIndicator(typingId);

        if (data.isError) {
            showError(data.errorMessage || 'An error occurred');
        } else {
            addMessage(data.text, 'assistant', data.usage);
            if (data.usage) {
                state.totalTokens += data.usage.totalTokens;
                updateStatistics();
            }
        }
    } catch (error) {
        removeTypingIndicator(typingId);
        throw error;
    }
}

// Handle streaming response using Server-Sent Events
async function handleStreamingResponse(request) {
    const messageElement = addMessage('', 'assistant', null, true);
    const messageTextElement = messageElement.querySelector('.message-text');
    let fullText = '';

    try {
        const response = await fetch('/Chat/StreamMessage', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        let buffer = '';

        while (true) {
            const { done, value } = await reader.read();

            if (done) {
                break;
            }

            // Decode the chunk
            buffer += decoder.decode(value, { stream: true });

            // Process complete SSE messages
            const lines = buffer.split('\n\n');
            buffer = lines.pop() || ''; // Keep incomplete message in buffer

            for (const line of lines) {
                if (line.startsWith('data: ')) {
                    const data = JSON.parse(line.slice(6));

                    if (data.type === 'delta' && data.text) {
                        fullText += data.text;
                        messageTextElement.textContent = fullText;
                        scrollToBottom();
                    } else if (data.type === 'usage' && data.usage) {
                        addUsageInfo(messageElement, data.usage);
                        state.totalTokens += data.usage.totalTokens;
                        updateStatistics();
                    } else if (data.type === 'error') {
                        showError(data.message || 'An error occurred during streaming');
                    } else if (data.type === 'done') {
                        // Streaming complete
                    }
                }
            }
        }
    } catch (error) {
        messageTextElement.textContent = 'Error: Failed to receive streaming response';
        throw error;
    }
}

// Add a message to the chat
function addMessage(text, role, usage = null, isStreaming = false) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message message-${role}`;

    const avatar = document.createElement('div');
    avatar.className = 'message-avatar';
    avatar.innerHTML = role === 'user' ? '<i class="bi bi-person-fill"></i>' : '<i class="bi bi-robot"></i>';

    const content = document.createElement('div');
    content.className = 'message-content';

    const textP = document.createElement('p');
    textP.className = 'message-text';
    textP.textContent = text;

    content.appendChild(textP);

    if (usage) {
        addUsageInfo(messageDiv, usage);
    }

    messageDiv.appendChild(avatar);
    messageDiv.appendChild(content);

    elements.messagesContainer.appendChild(messageDiv);
    scrollToBottom();

    return messageDiv;
}

// Add usage information to a message
function addUsageInfo(messageElement, usage) {
    let meta = messageElement.querySelector('.message-meta');
    if (!meta) {
        meta = document.createElement('div');
        meta.className = 'message-meta';
        const content = messageElement.querySelector('.message-content');
        content.appendChild(meta);
    }

    meta.innerHTML = `
        <i class="bi bi-bar-chart-fill"></i>
        Tokens: ${usage.inputTokens} in, ${usage.outputTokens} out, ${usage.totalTokens} total
    `;
}

// Show typing indicator
function showTypingIndicator() {
    const typingId = 'typing-' + Date.now();
    const typingDiv = document.createElement('div');
    typingDiv.id = typingId;
    typingDiv.className = 'typing-indicator';
    typingDiv.innerHTML = `
        <i class="bi bi-robot"></i>
        <span>AI is thinking</span>
        <div class="typing-dots">
            <div class="typing-dot"></div>
            <div class="typing-dot"></div>
            <div class="typing-dot"></div>
        </div>
    `;

    elements.messagesContainer.appendChild(typingDiv);
    scrollToBottom();

    return typingId;
}

// Remove typing indicator
function removeTypingIndicator(typingId) {
    const element = document.getElementById(typingId);
    if (element) {
        element.remove();
    }
}

// Remove welcome message
function removeWelcomeMessage() {
    const welcomeMessage = elements.messagesContainer.querySelector('.welcome-message');
    if (welcomeMessage) {
        welcomeMessage.remove();
    }
}

// Show error message
function showError(message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.innerHTML = `
        <i class="bi bi-exclamation-triangle-fill me-2"></i>
        <strong>Error:</strong> ${message}
    `;

    elements.messagesContainer.appendChild(errorDiv);
    scrollToBottom();

    // Auto-remove after 5 seconds
    setTimeout(() => {
        errorDiv.remove();
    }, 5000);
}

// Clear chat
function clearChat() {
    if (confirm('Are you sure you want to clear the chat history?')) {
        elements.messagesContainer.innerHTML = `
            <div class="welcome-message text-center">
                <i class="bi bi-robot welcome-icon"></i>
                <h2>Welcome to AI Chat!</h2>
                <p class="text-muted">
                    Start a conversation by typing a message below.
                    <br>
                    Choose between regular or streaming responses.
                </p>
            </div>
        `;

        state.messageCount = 0;
        state.totalTokens = 0;
        updateStatistics();
    }
}

// Update statistics display
function updateStatistics() {
    elements.messageCountDisplay.textContent = state.messageCount;
    elements.totalTokensDisplay.textContent = state.totalTokens;
}

// Set processing state (disable/enable controls)
function setProcessingState(isProcessing) {
    state.isProcessing = isProcessing;
    elements.sendButton.disabled = isProcessing;
    elements.messageInput.disabled = isProcessing;
    elements.clearButton.disabled = isProcessing;

    if (isProcessing) {
        elements.sendButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';
    } else {
        elements.sendButton.innerHTML = '<i class="bi bi-send-fill"></i> Send';
        elements.messageInput.focus();
    }
}

// Scroll to bottom of messages
function scrollToBottom() {
    elements.messagesContainer.scrollTop = elements.messagesContainer.scrollHeight;
}
