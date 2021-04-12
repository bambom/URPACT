using System;

public struct HitData
{
    public uint Target;
    public short HitX;
    public short HitY;
    public short HitZ;
    public short HitDir;
    public short StraightTime;
    public short LashX;
    public short LashY;
    public short LashZ;
    public short LashTime;
    public byte HitAction;
    public byte AttackLevel;

    public void Serialize(CommandSerializer ser)
    {
        ser.Ser(ref Target);
        ser.Ser(ref HitX);
        ser.Ser(ref HitY);
        ser.Ser(ref HitZ);
        ser.Ser(ref HitDir);
        ser.Ser(ref StraightTime);
        ser.Ser(ref LashX);
        ser.Ser(ref LashY);
        ser.Ser(ref LashZ);
        ser.Ser(ref LashTime);
        ser.Ser(ref HitAction);
        ser.Ser(ref AttackLevel);
    }
}

public class CommonData
{
	public const int 	DropItemMax = 9;
	public const int    UserMaxLevel = 80;
	public const string GradeLevelS	= "Icon07_Battle_Levle_01";
	public const string GradeLevelA = "Icon07_Battle_Levle_02";
	public const string GradeLevelB = "Icon07_Battle_Levle_03";
	public const string GradeLevelC = "Icon07_Battle_Levle_04";
	public const string GradeLevelD = "Icon07_Battle_Levle_05";
}
