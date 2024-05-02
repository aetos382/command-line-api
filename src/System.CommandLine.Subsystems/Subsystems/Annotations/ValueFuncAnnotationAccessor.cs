﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace System.CommandLine.Subsystems.Annotations;

/// <summary>
/// Associates an annotation with a <see cref="CliSymbol"/>. The symbol must be an option or argument and the delegate must return a value of the same type as the symbol./>. 
/// </summary>
/// <remarks>
/// The annotation will be stored by the accessor's owner <see cref="CliSubsystem"/>. 
/// </remarks>
/// <typeparam name="TValue">The type of value to be stored</typeparam>
/// <param name="owner">The subsystem that this annotation store data for.</param>
/// <param name="id">The identifier for this annotation, since subsystems may have multiple annotations.</param>
public struct ValueFuncAnnotationAccessor<TValue>(CliSubsystem owner, AnnotationId<Func<TValue>> id)
{
    /// <inheritdoc cref="AnnotationAccessor{TValue}.Id"/>>
    public AnnotationId<Func<TValue>> Id { get; }

    /// <inheritdoc cref="AnnotationAccessor{TValue}.Set"/>>
    public readonly void Set<TSymbolValue>(CliOption<TSymbolValue> symbol, Func<TValue> value)
        where TSymbolValue : TValue
        => owner.SetAnnotation(symbol, id, value);

    /// <inheritdoc cref="AnnotationAccessor{TValue}.Set"/>>
    public readonly void Set<TSymbolValue>(CliArgument<TSymbolValue> symbol, Func<TValue> value)
        where TSymbolValue : TValue
        => owner.SetAnnotation(symbol, id, value);

    // TODO: Consider whether we need a version that takes a CliSymbol (ValueSymbol)
    /// <inheritdoc cref="AnnotationAccessor{TValue}.Get"/>>
    public readonly bool TryGet<TSymbolValue>(CliOption<TSymbolValue> symbol, [NotNullWhen(true)] out Func<TValue>? value)
        where TSymbolValue : TValue
        => TryGetInternal<TSymbolValue>(symbol, out value);

    /// <inheritdoc cref="AnnotationAccessor{TValue}.Get"/>>
    public readonly bool TryGet<TSymbolValue>(CliArgument<TSymbolValue> symbol, [NotNullWhen(true)] out Func<TValue>? value)
        where TSymbolValue : TValue
        => TryGetInternal<TSymbolValue>(symbol, out value);

    /// <inheritdoc cref="AnnotationAccessor{TValue}.Get"/>>
    /// <remarks>
    /// This overload will throw if the stored value cannot be converted to the type.
    /// </remarks>
    /// <exception cref="InvalidCastException"/>
    public readonly bool TryGet<TSymbolValue>(CliSymbol symbol, [NotNullWhen(true)] out Func<TValue>? value)
        where TSymbolValue : TValue
        => TryGetInternal<TSymbolValue>(symbol, out value);

    private readonly bool TryGetInternal<TSymbolValue>(CliSymbol symbol, [NotNullWhen(true)] out Func<TValue>? value)
        where TSymbolValue : TValue
    {
        if (owner.TryGetAnnotation(symbol, id, out Func<TValue>? storedValue))
        {
            value = storedValue;
            return true;
        }
        value = default;
        return false;
    }
}
