using System.IO;

using UnityEngine;

public static class BinaryIOExtensions
{
    #region Vector2 + Vector2Int

    public static void Write(this BinaryWriter writer, Vector2 vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
    }

    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2(reader.ReadSingle(),
                           reader.ReadSingle());
    }

    public static void Write(this BinaryWriter writer, Vector2Int vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
    }

    public static Vector2Int ReadVector2Int(this BinaryReader reader)
    {
        return new Vector2Int(reader.ReadInt32(),
                              reader.ReadInt32());
    }

    #endregion

    #region Vector3 + Vector3Int

    public static void Write(this BinaryWriter writer, Vector3 vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
        writer.Write(vector.z);
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        return new Vector3(reader.ReadSingle(),
                           reader.ReadSingle(),
                           reader.ReadSingle());
    }

    public static void Write(this BinaryWriter writer, Vector3Int vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
        writer.Write(vector.z);
    }

    public static Vector3Int ReadVector3Int(this BinaryReader reader)
    {
        return new Vector3Int(reader.ReadInt32(),
                              reader.ReadInt32(),
                              reader.ReadInt32());
    }

    #endregion

    #region Quaternion

    public static void Write(this BinaryWriter writer, Quaternion quaternion)
    {
        writer.Write(quaternion.x);
        writer.Write(quaternion.y);
        writer.Write(quaternion.z);
        writer.Write(quaternion.w);
    }

    public static Quaternion ReadQuaternion(this BinaryReader reader)
    {
        return new Quaternion(reader.ReadSingle(),
                              reader.ReadSingle(),
                              reader.ReadSingle(),
                              reader.ReadSingle());
    }

    #endregion
}
