#region Using Directives

using System;

#endregion

namespace BibleReading.Common45.Root.Data.Nullable
{
    public interface INullableReader
    {

        #region Interface Methods

        bool GetBoolean(string name);
        Nullable<bool> GetNullableBoolean(string name);
        byte GetByte(string name);
        Nullable<byte> GetNullableByte(string name);
        char GetChar(string name);
        Nullable<char> GetNullableChar(string name);
        DateTime GetDateTime(string name);
        Nullable<DateTime> GetNullableDateTime(string name);
        decimal GetDecimal(string name);
        Nullable<Decimal> GetNullableDecimal(string name);
        double GetDouble(string name);
        Nullable<double> GetNullableDouble(string name);
        float GetFloat(string name);
        Nullable<float> GetNullableFloat(string name);
        Guid GetGuid(string name);
        Nullable<Guid> GetNullableGuid(string name);
        short GetInt16(string name);
        Nullable<short> GetNullableInt16(string name);
        int GetInt32(string name);
        Nullable<int> GetNullableInt32(string name);
        long GetInt64(string name);
        Nullable<long> GetNullableInt64(string name);
        string GetString(string name);
        string GetNullableString(string name);
        object GetValue(string name);
        bool IsDBNull(string name);

        #endregion

    }
}
