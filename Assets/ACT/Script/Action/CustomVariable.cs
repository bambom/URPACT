using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CustomVariable
{
    public int Value;
    public int Max;

    public void Adjust(int v)
    {
        int nValue = Value + v;
        Value = Mathf.Clamp(nValue, 0, Max);
    }

    public void Set(int v, int m)
    {
        Value = v;
        Max = m;
    }

    public bool Compare(ECompareType com, int v)
    {
        return Compare(com, Value, v);
    }

    public static bool Compare(ECompareType com, int a, int v)
    {
        switch (com)
        {
            case ECompareType.ECT_EQUAL: return a == v; // 等于（==）
            case ECompareType.ECT_NOT_EUQAL: return a != v; // 不等于（！=）
            case ECompareType.ECT_GREATER: return a > v; // 大于（>）
            case ECompareType.ECT_LESS: return a < v; // 小于（<）
            case ECompareType.ECT_GREATER_EQUAL: return a >= v;// 大于或等于（>=）
            case ECompareType.ECT_LESS_EQUAL: return a <= v;// 小于或等于（<=）
            default: return false;
        }
    }
}
