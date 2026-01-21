#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member - these are model ID constants

namespace AiSdk.Providers.Gateway.Models;

/// <summary>
/// Available Gateway language model IDs.
/// </summary>
public static class GatewayLanguageModelIds
{
    // Alibaba
    public const string Qwen3_14b = "alibaba/qwen-3-14b";
    public const string Qwen3_235b = "alibaba/qwen-3-235b";
    public const string Qwen3_30b = "alibaba/qwen-3-30b";
    public const string Qwen3_32b = "alibaba/qwen-3-32b";
    public const string Qwen3_235b_A22b_Thinking = "alibaba/qwen3-235b-a22b-thinking";
    public const string Qwen3_Coder = "alibaba/qwen3-coder";
    public const string Qwen3_Coder_30b_A3b = "alibaba/qwen3-coder-30b-a3b";
    public const string Qwen3_Coder_Plus = "alibaba/qwen3-coder-plus";
    public const string Qwen3_Max = "alibaba/qwen3-max";
    public const string Qwen3_Max_Preview = "alibaba/qwen3-max-preview";
    public const string Qwen3_Next_80b_A3b_Instruct = "alibaba/qwen3-next-80b-a3b-instruct";
    public const string Qwen3_Next_80b_A3b_Thinking = "alibaba/qwen3-next-80b-a3b-thinking";
    public const string Qwen3_Vl_Instruct = "alibaba/qwen3-vl-instruct";
    public const string Qwen3_Vl_Thinking = "alibaba/qwen3-vl-thinking";

    // Amazon
    public const string Amazon_Nova_2_Lite = "amazon/nova-2-lite";
    public const string Amazon_Nova_Lite = "amazon/nova-lite";
    public const string Amazon_Nova_Micro = "amazon/nova-micro";
    public const string Amazon_Nova_Pro = "amazon/nova-pro";

    // Anthropic
    public const string Claude3_Haiku = "anthropic/claude-3-haiku";
    public const string Claude3_Opus = "anthropic/claude-3-opus";
    public const string Claude35_Haiku = "anthropic/claude-3.5-haiku";
    public const string Claude35_Sonnet = "anthropic/claude-3.5-sonnet";
    public const string Claude35_Sonnet_20240620 = "anthropic/claude-3.5-sonnet-20240620";
    public const string Claude37_Sonnet = "anthropic/claude-3.7-sonnet";
    public const string Claude_Haiku_4_5 = "anthropic/claude-haiku-4.5";
    public const string Claude_Opus_4 = "anthropic/claude-opus-4";
    public const string Claude_Opus_4_1 = "anthropic/claude-opus-4.1";
    public const string Claude_Opus_4_5 = "anthropic/claude-opus-4.5";
    public const string Claude_Sonnet_4 = "anthropic/claude-sonnet-4";
    public const string Claude_Sonnet_4_5 = "anthropic/claude-sonnet-4.5";

    // OpenAI
    public const string Gpt3_5_Turbo = "openai/gpt-3.5-turbo";
    public const string Gpt3_5_Turbo_Instruct = "openai/gpt-3.5-turbo-instruct";
    public const string Gpt4_Turbo = "openai/gpt-4-turbo";
    public const string Gpt4_1 = "openai/gpt-4.1";
    public const string Gpt4_1_Mini = "openai/gpt-4.1-mini";
    public const string Gpt4_1_Nano = "openai/gpt-4.1-nano";
    public const string Gpt4o = "openai/gpt-4o";
    public const string Gpt4o_Mini = "openai/gpt-4o-mini";
    public const string Gpt5 = "openai/gpt-5";
    public const string Gpt5_Chat = "openai/gpt-5-chat";
    public const string Gpt5_Codex = "openai/gpt-5-codex";
    public const string Gpt5_Mini = "openai/gpt-5-mini";
    public const string Gpt5_Nano = "openai/gpt-5-nano";
    public const string Gpt5_Pro = "openai/gpt-5-pro";
    public const string Gpt5_1_Codex = "openai/gpt-5.1-codex";
    public const string Gpt5_1_Codex_Max = "openai/gpt-5.1-codex-max";
    public const string Gpt5_1_Codex_Mini = "openai/gpt-5.1-codex-mini";
    public const string Gpt5_1_Instant = "openai/gpt-5.1-instant";
    public const string Gpt5_1_Thinking = "openai/gpt-5.1-thinking";
    public const string Gpt5_2 = "openai/gpt-5.2";
    public const string Gpt5_2_Chat = "openai/gpt-5.2-chat";
    public const string Gpt5_2_Pro = "openai/gpt-5.2-pro";
    public const string GptOss_120b = "openai/gpt-oss-120b";
    public const string GptOss_20b = "openai/gpt-oss-20b";
    public const string GptOss_Safeguard_20b = "openai/gpt-oss-safeguard-20b";
    public const string O1 = "openai/o1";
    public const string O3 = "openai/o3";
    public const string O3_Deep_Research = "openai/o3-deep-research";
    public const string O3_Mini = "openai/o3-mini";
    public const string O3_Pro = "openai/o3-pro";
    public const string O4_Mini = "openai/o4-mini";

    // Google
    public const string Gemini_2_0_Flash = "google/gemini-2.0-flash";
    public const string Gemini_2_0_Flash_Lite = "google/gemini-2.0-flash-lite";
    public const string Gemini_2_5_Flash = "google/gemini-2.5-flash";
    public const string Gemini_2_5_Flash_Image = "google/gemini-2.5-flash-image";
    public const string Gemini_2_5_Flash_Image_Preview = "google/gemini-2.5-flash-image-preview";
    public const string Gemini_2_5_Flash_Lite = "google/gemini-2.5-flash-lite";
    public const string Gemini_2_5_Flash_Lite_Preview_09_2025 = "google/gemini-2.5-flash-lite-preview-09-2025";
    public const string Gemini_2_5_Flash_Preview_09_2025 = "google/gemini-2.5-flash-preview-09-2025";
    public const string Gemini_2_5_Pro = "google/gemini-2.5-pro";
    public const string Gemini_3_Flash = "google/gemini-3-flash";
    public const string Gemini_3_Pro_Image = "google/gemini-3-pro-image";
    public const string Gemini_3_Pro_Preview = "google/gemini-3-pro-preview";

    // Meta
    public const string Llama3_1_70b = "meta/llama-3.1-70b";
    public const string Llama3_1_8b = "meta/llama-3.1-8b";
    public const string Llama3_2_11b = "meta/llama-3.2-11b";
    public const string Llama3_2_1b = "meta/llama-3.2-1b";
    public const string Llama3_2_3b = "meta/llama-3.2-3b";
    public const string Llama3_2_90b = "meta/llama-3.2-90b";
    public const string Llama3_3_70b = "meta/llama-3.3-70b";
    public const string Llama4_Maverick = "meta/llama-4-maverick";
    public const string Llama4_Scout = "meta/llama-4-scout";

    // DeepSeek
    public const string DeepSeek_R1 = "deepseek/deepseek-r1";
    public const string DeepSeek_V3 = "deepseek/deepseek-v3";
    public const string DeepSeek_V3_1 = "deepseek/deepseek-v3.1";
    public const string DeepSeek_V3_1_Terminus = "deepseek/deepseek-v3.1-terminus";
    public const string DeepSeek_V3_2 = "deepseek/deepseek-v3.2";
    public const string DeepSeek_V3_2_Exp = "deepseek/deepseek-v3.2-exp";
    public const string DeepSeek_V3_2_Thinking = "deepseek/deepseek-v3.2-thinking";

    // Mistral
    public const string Codestral = "mistral/codestral";
    public const string Devstral_2 = "mistral/devstral-2";
    public const string Devstral_Small = "mistral/devstral-small";
    public const string Devstral_Small_2 = "mistral/devstral-small-2";
    public const string Magistral_Medium = "mistral/magistral-medium";
    public const string Magistral_Small = "mistral/magistral-small";
    public const string Ministral_14b = "mistral/ministral-14b";
    public const string Ministral_3b = "mistral/ministral-3b";
    public const string Ministral_8b = "mistral/ministral-8b";
    public const string Mistral_Large_3 = "mistral/mistral-large-3";
    public const string Mistral_Medium = "mistral/mistral-medium";
    public const string Mistral_Nemo = "mistral/mistral-nemo";
    public const string Mistral_Small = "mistral/mistral-small";
    public const string Mixtral_8x22b_Instruct = "mistral/mixtral-8x22b-instruct";
    public const string Pixtral_12b = "mistral/pixtral-12b";
    public const string Pixtral_Large = "mistral/pixtral-large";

    // Cohere
    public const string Command_A = "cohere/command-a";

    // Perplexity
    public const string Sonar = "perplexity/sonar";
    public const string Sonar_Pro = "perplexity/sonar-pro";
    public const string Sonar_Reasoning = "perplexity/sonar-reasoning";
    public const string Sonar_Reasoning_Pro = "perplexity/sonar-reasoning-pro";

    // xAI
    public const string Grok_2 = "xai/grok-2";
    public const string Grok_2_Vision = "xai/grok-2-vision";
    public const string Grok_3 = "xai/grok-3";
    public const string Grok_3_Fast = "xai/grok-3-fast";
    public const string Grok_3_Mini = "xai/grok-3-mini";
    public const string Grok_3_Mini_Fast = "xai/grok-3-mini-fast";
    public const string Grok_4 = "xai/grok-4";
    public const string Grok_4_Fast_Non_Reasoning = "xai/grok-4-fast-non-reasoning";
    public const string Grok_4_Fast_Reasoning = "xai/grok-4-fast-reasoning";
    public const string Grok_4_1_Fast_Non_Reasoning = "xai/grok-4.1-fast-non-reasoning";
    public const string Grok_4_1_Fast_Reasoning = "xai/grok-4.1-fast-reasoning";
    public const string Grok_Code_Fast_1 = "xai/grok-code-fast-1";

    // Zai
    public const string GLM_4_5 = "zai/glm-4.5";
    public const string GLM_4_5_Air = "zai/glm-4.5-air";
    public const string GLM_4_5v = "zai/glm-4.5v";
    public const string GLM_4_6 = "zai/glm-4.6";
    public const string GLM_4_6v = "zai/glm-4.6v";
    public const string GLM_4_6v_Flash = "zai/glm-4.6v-flash";
    public const string GLM_4_7 = "zai/glm-4.7";

    // Vercel
    public const string V0_1_0_Md = "vercel/v0-1.0-md";
    public const string V0_1_5_Md = "vercel/v0-1.5-md";

    // Additional models (partial list - see TypeScript source for complete list)
    public const string Bytedance_Seed_1_6 = "bytedance/seed-1.6";
    public const string Meituan_Longcat_Flash_Chat = "meituan/longcat-flash-chat";
    public const string Meituan_Longcat_Flash_Thinking = "meituan/longcat-flash-thinking";
    public const string Minimax_Minimax_M2 = "minimax/minimax-m2";
    public const string Minimax_Minimax_M2_1 = "minimax/minimax-m2.1";
    public const string Minimax_Minimax_M2_1_Lightning = "minimax/minimax-m2.1-lightning";
    public const string Moonshotai_Kimi_K2 = "moonshotai/kimi-k2";
    public const string Moonshotai_Kimi_K2_0905 = "moonshotai/kimi-k2-0905";
    public const string Moonshotai_Kimi_K2_Thinking = "moonshotai/kimi-k2-thinking";
    public const string Moonshotai_Kimi_K2_Thinking_Turbo = "moonshotai/kimi-k2-thinking-turbo";
    public const string Moonshotai_Kimi_K2_Turbo = "moonshotai/kimi-k2-turbo";
    public const string Morph_Morph_V3_Fast = "morph/morph-v3-fast";
    public const string Morph_Morph_V3_Large = "morph/morph-v3-large";
    public const string Nvidia_Nemotron_3_Nano_30b_A3b = "nvidia/nemotron-3-nano-30b-a3b";
    public const string Nvidia_Nemotron_Nano_12b_V2_Vl = "nvidia/nemotron-nano-12b-v2-vl";
    public const string Nvidia_Nemotron_Nano_9b_V2 = "nvidia/nemotron-nano-9b-v2";
    public const string Arcee_Trinity_Mini = "arcee-ai/trinity-mini";
    public const string Inception_Mercury_Coder_Small = "inception/mercury-coder-small";
    public const string Kwaipilot_Kat_Coder_Pro_V1 = "kwaipilot/kat-coder-pro-v1";
    public const string Prime_Intellect_Intellect_3 = "prime-intellect/intellect-3";
    public const string Stealth_Sonoma_Dusk_Alpha = "stealth/sonoma-dusk-alpha";
    public const string Stealth_Sonoma_Sky_Alpha = "stealth/sonoma-sky-alpha";
    public const string Xiaomi_Mimo_V2_Flash = "xiaomi/mimo-v2-flash";
    public const string Openai_Codex_Mini = "openai/codex-mini";
}
