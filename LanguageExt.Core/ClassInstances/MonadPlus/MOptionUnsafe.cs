﻿using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct MOptionUnsafe<A> :
        Choice<OptionUnsafe<A>, Unit, A>,
        MonadPlus<OptionUnsafe<A>, A>,
        Optional<OptionUnsafe<A>, A>,
        Foldable<OptionUnsafe<A>, A>,
        BiFoldable<OptionUnsafe<A>, Unit, A>,
        Liftable<OptionUnsafe<A>, A>
    {
        public static readonly MOptionUnsafe<A> Inst = default(MOptionUnsafe<A>);

        [Pure]
        public MB Bind<MONADB, MB, B>(OptionUnsafe<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B>
        {
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ma.IsSome && f != null
                ? f(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);
        }

        [Pure]
        public OptionUnsafe<A> Fail(object err) =>
            OptionUnsafe<A>.None;

        [Pure]
        public OptionUnsafe<A> Fail(Exception err = null) =>
            OptionUnsafe<A>.None;

        [Pure]
        public OptionUnsafe<A> Plus(OptionUnsafe<A> a, OptionUnsafe<A> b) =>
            a.IsSome
                ? a
                : b;

        [Pure]
        public OptionUnsafe<A> FromSeq(IEnumerable<A> xs)
        {
            if (xs == null) return None;
            var x = xs.Take(1).ToArray();
            return x.Length == 0
                ? OptionUnsafe<A>.None
                : Return(x[0]);
        }

        [Pure]
        public OptionUnsafe<A> Return(A x) =>
            new OptionUnsafe<A>(new SomeValue<A>(x));

        [Pure]
        public OptionUnsafe<A> Return(Func<A> f) =>
            Return(f());

        [Pure]
        public OptionUnsafe<A> Zero() =>
            OptionUnsafe<A>.None;

        [Pure]
        public bool IsNone(OptionUnsafe<A> opt) =>
            opt.IsNone;

        [Pure]
        public bool IsSome(OptionUnsafe<A> opt) =>
            opt.IsSome;

        [Pure]
        public bool IsUnsafe(OptionUnsafe<A> opt) =>
            true;

        [Pure]
        public B Match<B>(OptionUnsafe<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Some(opt.Value)
                : None();
        }

        public Unit Match(OptionUnsafe<A> opt, Action<A> Some, Action None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            if (opt.IsSome) Some(opt.Value); else None();
            return unit;
        }

        [Pure]
        public B MatchUnsafe<B>(OptionUnsafe<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Some(opt.Value)
                : None();
        }

        [Pure]
        public S Fold<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> f)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ma.IsSome
                ? f(state, ma.Value)
                : state;
        }

        [Pure]
        public S FoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> f)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ma.IsSome
                ? f(state, ma.Value)
                : state;
        }

        [Pure]
        public S BiFold<S>(OptionUnsafe<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return ma.IsNone
                ? fa(state, unit)
                : fb(state, ma.Value);
        }

        [Pure]
        public S BiFoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return ma.IsNone
                ? fa(state, unit)
                : fb(state, ma.Value);
        }

        [Pure]
        public int Count(OptionUnsafe<A> ma) =>
            ma.IsSome
                ? 1
                : 0;

        [Pure]
        public OptionUnsafe<A> Lift(A x) =>
            Return(x);

        [Pure]
        public bool IsChoice1(OptionUnsafe<A> choice) =>
            choice.IsNone;

        [Pure]
        public bool IsChoice2(OptionUnsafe<A> choice) =>
            choice.IsSome;

        [Pure]
        public bool IsBottom(OptionUnsafe<A> choice) =>
            false;

        [Pure]
        public C Match<C>(OptionUnsafe<A> choice, Func<Unit, C> Choice1, Func<A, C> Choice2, Func<C> Bottom = null) =>
            choice.MatchUnsafe(
                Some: Choice2,
                None: () => Choice1(unit));

        [Pure]
        public Unit Match(OptionUnsafe<A> choice, Action<Unit> Choice1, Action<A> Choice2, Action Bottom = null) =>
            choice.MatchUnsafe(
                Some: Choice2,
                None: () => Choice1(unit));

        [Pure]
        public C MatchUnsafe<C>(OptionUnsafe<A> choice, Func<Unit, C> Choice1, Func<A, C> Choice2, Func<C> Bottom = null) =>
            choice.MatchUnsafe(
                Some: Choice2,
                None: () => Choice1(unit));
    }
}
