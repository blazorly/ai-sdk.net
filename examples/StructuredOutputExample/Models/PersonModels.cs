namespace StructuredOutputExample.Models;

/// <summary>
/// Represents a person's profile information.
/// </summary>
public class PersonProfile
{
    /// <summary>
    /// Full name of the person.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Age in years.
    /// </summary>
    public required int Age { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Contact phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Physical address.
    /// </summary>
    public required Address Address { get; set; }

    /// <summary>
    /// Professional information.
    /// </summary>
    public required Professional Professional { get; set; }

    /// <summary>
    /// List of hobbies and interests.
    /// </summary>
    public List<string>? Hobbies { get; set; }

    /// <summary>
    /// Social media profiles.
    /// </summary>
    public Dictionary<string, string>? SocialMedia { get; set; }
}

/// <summary>
/// Represents a physical address.
/// </summary>
public class Address
{
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
}

/// <summary>
/// Represents professional information.
/// </summary>
public class Professional
{
    public required string Title { get; set; }
    public required string Company { get; set; }
    public required string Industry { get; set; }
    public required int YearsOfExperience { get; set; }
    public List<string>? Skills { get; set; }
}
