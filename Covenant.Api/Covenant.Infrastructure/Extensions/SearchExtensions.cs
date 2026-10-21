using System.Linq.Expressions;
using System.Reflection;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Covenant.Infrastructure.Extensions;

public static class SearchExtensions
{
    private static readonly MethodInfo LikeMethod = typeof(DbFunctionsExtensions)
        .GetMethod(nameof(DbFunctionsExtensions.Like), [typeof(DbFunctions), typeof(string), typeof(string), typeof(string)]);

    private static readonly Expression EscapeCharacter = Expression.Constant(@"\");

    private static readonly MethodInfo UnaccentLowerMethod = typeof(PostgresFunctions)
        .GetMethod(nameof(PostgresFunctions.unaccent_lower));

    private static readonly MethodInfo WordSimilarityMethod = typeof(PostgresFunctions)
        .GetMethod(nameof(PostgresFunctions.word_similarity));

    private static readonly Expression Functions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

    public static IQueryable<T> WhereSearch<T>(this IQueryable<T> query, string term, params Expression<Func<T, string>>[] fields) =>
        HasSearchTokens(term) ? query.Where(SearchPredicate(term, fields)) : query;

    public static Expression<Func<T, bool>> AndSearch<T>(this Expression<Func<T, bool>> predicate, string term, params Expression<Func<T, string>>[] fields) =>
        HasSearchTokens(term) ? predicate.And(SearchPredicate(term, fields)) : predicate;

    public static bool HasSearchTokens(string term) => term.ToSearchTokens().Length > 0;

    public static Expression<Func<T, bool>> SearchPredicate<T>(string term, params Expression<Func<T, string>>[] fields)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var tokens = term.ToSearchTokens();
        if (tokens.Length == 0 || fields.Length == 0) return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), parameter);

        var body = tokens
            .Select(token => fields
                .Select(field => MatchesToken(ReplacingExpressionVisitor.Replace(field.Parameters[0], parameter, field.Body), token))
                .Aggregate(Expression.OrElse))
            .Aggregate(Expression.AndAlso);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    public static Expression<Func<T, bool>> SkillSearchPredicate<T>(this CovenantContext context, string term, Expression<Func<T, string>> skill)
    {
        var key = term.ToComparisonKey();
        Expression<Func<string, bool>> isCatalogMatch = value => context.Skills.Any(s =>
            s.Value == value && (PostgresFunctions.unaccent_lower(s.Value) == key || s.Aliases.Contains(key)));
        var catalogMatch = Expression.Lambda<Func<T, bool>>(
            ReplacingExpressionVisitor.Replace(isCatalogMatch.Parameters[0], skill.Body, isCatalogMatch.Body),
            skill.Parameters);
        return SearchPredicate(term, skill).Or(catalogMatch);
    }

    public static Expression<Func<T, bool>> SimilarityPredicate<T>(string term, double threshold, params Expression<Func<T, string>>[] fields)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var normalizedTerm = string.Join(' ', term.ToSearchTokens());
        Expression<Func<string>> termParameter = () => normalizedTerm;
        Expression<Func<double>> thresholdParameter = () => threshold;
        var body = fields
            .Select(field => (Expression)Expression.GreaterThanOrEqual(
                Expression.Call(WordSimilarityMethod, termParameter.Body,
                    Expression.Call(UnaccentLowerMethod, ReplacingExpressionVisitor.Replace(field.Parameters[0], parameter, field.Body))),
                thresholdParameter.Body))
            .Aggregate(Expression.OrElse);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression MatchesToken(Expression field, string token)
    {
        var pattern = $"%{EscapeLike(token)}%";
        Expression<Func<string>> patternParameter = () => pattern;
        return Expression.Call(LikeMethod, Functions, Expression.Call(UnaccentLowerMethod, field), patternParameter.Body, EscapeCharacter);
    }

    private static string EscapeLike(string value) =>
        value.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
}
