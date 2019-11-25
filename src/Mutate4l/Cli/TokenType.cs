﻿namespace Mutate4l.Cli
{
    public enum TokenType
    {
        NoToken,

        _CommandsBegin,
        Arpeggiate,
        Concat,
        Crop,
        Filter,
        Interleave,
        InterleaveEvent,
        Legato,
        Mask,
        Monophonize,
        Padding,
        Quantize,
        Ratchet,
        Relength,
        Remap,
        Resize,
        Scan,
        SetLength,
        SetPitch,
        SetRhythm,
        Scale,
        Shuffle,
        Slice,
        Sustain,
        Take,
        Transpose,

        _OptionsBegin,
        Skip,     // Need to find a better way of supporting token names that can signify both options and commands. Time to stop using an enum for this I guess. For now we have this quick fix though. 
        _CommandsEnd,
        Amount,
        AutoScale,
        By,
        ChunkChords,
        ControlMax,
        ControlMin,
        Count,
        Divisions,
        Duration,
        EnableMask,
        Factor,
        Invert,
        Length,
        Lengths,
        Magnetic,
        Max,
        Min,
        Mode,
        PadAmount,
        Pitch,
        PitchValues,
        Post,
        Ranges,
        RatchetValues,
        RemoveOffset,
        Repeats,
        Rescale,
        Shape,
        ShuffleValues,
        SkipCounts,
        Solo,
        Start,
        Strength,
        Strict,
        TakeCounts,
        TransposeValues,
        Threshold,
        To,
        VelocityToStrength,
        Window,
        With,
        _OptionsEnd,

        _EnumValuesBegin,
        Absolute,
        Both,
        EaseIn,
        EaseInOut,
        EaseOut,
        Event,
        Linear,
        Overwrite,
        Velocity,
        Pitches, // todo: Quickfix to avoid conflict with Pitch. Need to find a better solution here...
        Relative,
        Rhythm,
        Time,
        _EnumValuesEnd,

        _ValuesBegin,
        ClipReference,
        Decimal,
        InlineClip,
        MusicalDivision,
        BarsBeatsSixteenths,
        Number,
        _ValuesEnd,

        _OperatorsBegin,
        RangeOperator,
        AlternationOperator,
        EmptyOperator,
        RepeatOperator,
        FillOperator,
        _OperatorsEnd,

        _TestOptionsBegin,
        DecimalValue,
        DecimalValues,
        IntValue,
        IntValues,
        EnumValue,
        SimpleBoolFlag,
        _TestOptionsEnd,

        _TestEnumValuesBegin,
        EnumValue1,
        EnumValue2,
        _TestEnumValuesEnd
    }
}
