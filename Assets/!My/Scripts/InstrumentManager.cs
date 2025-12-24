using System;
using System.Collections;
using UnityEngine;

public enum InstrumentType { None, Create, HideWall }
public class InstrumentManager
{
    public Action<InstrumentType> OnChangeInstrument;

    public InstrumentType Instrument { get; private set; } = InstrumentType.Create;

    public void SetInstrument(InstrumentType instrumentType)
    {
        if(Instrument == instrumentType)
        {
            if (Instrument != InstrumentType.None)
                SetInstrument(InstrumentType.None);
            return;
        }

        Instrument = instrumentType;

        Debug.Log($"SetInstrument {instrumentType}");

        switch (instrumentType)
        {
            case InstrumentType.None:
                break;
            case InstrumentType.Create:
                break;
            case InstrumentType.HideWall:
                break;
            default:
                break;
        }

        OnChangeInstrument?.Invoke(instrumentType);
    }
}