using System.Numerics;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Sewer56.UI.Controller.Core;

/// <summary>
/// Obstructs all mathematical functions in the library.
/// </summary>
public static class Mathematics
{
    // TODO: Unroll the loops here [and AVX/SSE for CalculateVectors] if I ever feel like micro-optimising.

    /// <summary>
    /// Calculates the angle between two vectors.
    /// </summary>
    /// <param name="forwardVectorFirst">The first vector.</param>
    /// <param name="forwardVectorSecond">The second vector.</param>
    /// <returns>The angle in degrees.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static float CalcAngle(this in Vector2 forwardVectorFirst, in Vector2 forwardVectorSecond)
    {
        var dotProduct = Vector2.Dot(forwardVectorFirst, forwardVectorSecond);
        var magnitudes = forwardVectorFirst.Length() * forwardVectorSecond.Length();

        var cosTheta = dotProduct / (magnitudes);
        var arcCos = MathF.Acos(Math.Clamp(cosTheta, -1, 1));

        return (float)(arcCos / MathF.PI * 180.0);
    }

    /// <summary>
    /// Calculates a vector from the source to the target element.
    /// </summary>
    /// <param name="target">The target element.</param>
    /// <param name="source">The source element.</param>
    /// <returns>The vector from source to target</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static Vector2 CalculateVector(in Vector2 target, in Vector2 source) => target - source;

    /// <summary>
    /// Given a span with possible duplicates, selects all duplicates at the start of the span.
    /// </summary>
    /// <typeparam name="TElement">The element type.</typeparam>
    /// <typeparam name="TComparer">The comparer type.</typeparam>
    /// <param name="elements">The elements to get duplicates from.</param>
    /// <param name="comparer">The comparer used to check for duplicate.</param>
    public static Span<TElement> SliceLeadingDuplicates<TElement, TComparer>(Span<TElement> elements, TComparer comparer) where TComparer : IComparer<TElement>
    {
        // Just in case.
        if (elements.Length == 1)
            return elements;

        int x = 1;
        TElement duplicateElement = elements[0];

        for (; x < elements.Length; x++)
        {
            if (comparer.Compare(elements[x], duplicateElement) != 0)
                break;
        }
        
        return elements[..x];
    }
}