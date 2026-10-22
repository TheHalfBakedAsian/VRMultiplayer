using System;
using Unity.Netcode;
using Unity.XR.CompositionLayers.Layers;
[Serializable]
public struct QAresponses : INetworkSerializable, IEquatable<QAresponses>
{
    public string AnswerOne;
    public string AnswerTwo;
    public int AnswerThree;
    public QAresponses(string one, string two, int three)
    {
        AnswerOne = one;
        AnswerTwo = two;
        AnswerThree = three;
    }
    public string toString()
    {
        return AnswerOne +","+ AnswerTwo + "," + AnswerThree;
    }
    public bool Equals(QAresponses other)
    {
        return (AnswerOne == other.AnswerOne && AnswerTwo == other.AnswerTwo && AnswerThree == other.AnswerThree);
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref AnswerOne);
        serializer.SerializeValue(ref AnswerTwo);
        serializer.SerializeValue(ref AnswerThree);
    }
}