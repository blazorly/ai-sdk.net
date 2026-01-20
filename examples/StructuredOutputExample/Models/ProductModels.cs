namespace StructuredOutputExample.Models;

/// <summary>
/// Represents a product analysis.
/// </summary>
public class ProductAnalysis
{
    /// <summary>
    /// The product name.
    /// </summary>
    public required string ProductName { get; set; }

    /// <summary>
    /// Product category.
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Target audience description.
    /// </summary>
    public required string TargetAudience { get; set; }

    /// <summary>
    /// Key features of the product.
    /// </summary>
    public required List<string> KeyFeatures { get; set; }

    /// <summary>
    /// Identified strengths.
    /// </summary>
    public required List<string> Strengths { get; set; }

    /// <summary>
    /// Identified weaknesses.
    /// </summary>
    public required List<string> Weaknesses { get; set; }

    /// <summary>
    /// Competitive advantages.
    /// </summary>
    public required List<string> CompetitiveAdvantages { get; set; }

    /// <summary>
    /// Recommended pricing strategy.
    /// </summary>
    public required PricingStrategy Pricing { get; set; }

    /// <summary>
    /// Market opportunity score (1-10).
    /// </summary>
    public required int MarketOpportunityScore { get; set; }

    /// <summary>
    /// Overall recommendation.
    /// </summary>
    public required string Recommendation { get; set; }
}

/// <summary>
/// Represents a pricing strategy.
/// </summary>
public class PricingStrategy
{
    public required string Strategy { get; set; }
    public required decimal SuggestedPriceMin { get; set; }
    public required decimal SuggestedPriceMax { get; set; }
    public required string Reasoning { get; set; }
}
