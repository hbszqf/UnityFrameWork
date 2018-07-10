using System;
using System.Text;
using UnityEngine;

public class StringTool
{
    static StringBuilder stringBuilder = new StringBuilder();

    /// <summary>
    /// 拼接字符串,;
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns></returns>
    public static string combineString(string str1, string str2)
    {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(str1);
        stringBuilder.Append(str2);
        return stringBuilder.ToString ();
    }

    /// <summary>
    /// 拼接字符串,;
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns></returns>
    public static string combineString(string format, params object[] args)
    {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.AppendFormat(format, args);
        return stringBuilder.ToString();
    }

	///用*隐藏source字符串中的illegal特殊字符串,;
	public static string HideString(string source, string illegal)
	{
		string tStr = source;
		tStr.Replace (illegal,EncodeByLength(illegal,"*"));
		return tStr;
	}
		
	///将source字符串中的每个字符都用code字符串替代;
	public static string EncodeByLength(string source, string code)
	{
		string tStr = "";			
		for (int i = 0, iMax = source.Length - 1; i < iMax; i++) tStr += code;
		return tStr;
	}
		
    /// <summary>
    /// ff0000ff格式的颜色字符串转换成颜色Color对象.
    /// </summary>
    public static Color SexadecimalStringToColor(string color)
    {
        if (color == "" || color == null) return Color.white;
        char[] str = color.ToCharArray();
        float r = (float)System.Int32.Parse(str[0].ToString() + str[1].ToString(), System.Globalization.NumberStyles.HexNumber) / 255;
        float g = (float)System.Int32.Parse(str[2].ToString() + str[3].ToString(), System.Globalization.NumberStyles.HexNumber) / 255;
        float b = (float)System.Int32.Parse(str[4].ToString() + str[5].ToString(), System.Globalization.NumberStyles.HexNumber) / 255;
        float a = (float)System.Int32.Parse(str[6].ToString() + str[7].ToString(), System.Globalization.NumberStyles.HexNumber) / 255;
        return new Color(r, g, b, a);
    }

    /// <summary>
    /// (r,g,b,a)格式的字符串转换成颜色Color对象.
    /// </summary>
    public static Color StringToColor(string param)
    {
        string[] strArr = param.Split(',');
        int len = strArr.Length;
        float r = len > 0 ? float.Parse(strArr[0]) : 0;
        float g = len > 1 ? float.Parse(strArr[1]) : 0;
        float b = len > 2 ? float.Parse(strArr[2]) : 0;
        float a = len > 3 ? float.Parse(strArr[3]) : 0;
        return new Color(r, g, b, a);
    }

    /// <summary>
    /// (x,y,z)格式的字符串转换成Vector3对象.
    /// </summary>
    public static Vector3 StringToVector3(string param)
    {
        string[] strArr = param.Split(',');
        int len=strArr.Length;
        float x = len > 0 ? float.Parse(strArr[0]) : 0;
        float y = len > 1 ? float.Parse(strArr[1]) : 0;
        float z = len > 2 ? float.Parse(strArr[2]) : 0;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// (x,y,z,w)格式的字符串转换成Vector4对象.
    /// </summary>
    public static Vector4 StringToVector4(string param)
    {
        string[] strArr = param.Split(',');
        int len = strArr.Length;
        float x = len > 0 ? float.Parse(strArr[0]) : 0;
        float y = len > 1 ? float.Parse(strArr[1]) : 0;
        float z = len > 2 ? float.Parse(strArr[2]) : 0;
        float w = len > 3 ? float.Parse(strArr[3]) : 0;
        return new Vector4(x, y, z, w);
    }

    public static string ColorToString(Color param)
    {
        string str = param.r + "," + param.g + "," + param.b + "," + param.a;
        return str;
    }

    public static string Vector3ToString(Vector3 param)
    {
        string str = param.x.ToString("F4") + "," + param.y.ToString("F4") + "," + param.z.ToString("F4");
        return str;
    }

    public static string Vector4ToString(Vector4 param)
    {
        string str = param.x.ToString("F4") + "," + param.y.ToString("F4") + "," + param.z.ToString("F4") + "," + param.w.ToString("F4");
        return str;
    }
	
}

	

