using System;
using System.Collections.Generic;
using Lib.Extensions;

namespace Lib.ConvertingLib
{

    public delegate void SuccsesReadTag(TagRulsStruct tag, string row, string data);
    /// <summary>
    /// Если вернет true (прекратить работу)
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="index"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public delegate void ErrorReadTag(TagRulsStruct tag, string row, string data, string message, int indexRow);
    public enum TagRuls : byte
    {
        Один_Необязательный,
        Один_Обязательный,
        Неопределенное_количество,
        Закрывашка = 255
    }
    public struct TagRulsStruct
    {
        public string TagName { get; }
        public List<TagRulsStruct> Children { get; }
        //public string BeforeTagName { get; }
        public TagRuls Type { get; }
        public TagRulsStruct(string tagName)
        {
            this = new TagRulsStruct(tagName, null, TagRuls.Неопределенное_количество);
        }
        public TagRulsStruct(string tagName, List<TagRulsStruct> children)
        {
            this = new TagRulsStruct(tagName, children, TagRuls.Неопределенное_количество);
        }
        public TagRulsStruct(string tagName, TagRuls ruls)
        {
            this = new TagRulsStruct(tagName, null, ruls);
        }

        public TagRulsStruct(string tagName, List<TagRulsStruct> children, TagRuls ruls)
        {
            TagName = tagName;
            Type = ruls;
            Children = children;
        }
    }
    public interface IConverting
    {
        string[] StrToRows(string content);
        string RowsToStr(string[] rows);
        string FormatTag(string tagName);//root
        string FormatTag(string tagName, string rowContent);
        void ConwertingRead(List<TagRulsStruct> tags, string[] rows, SuccsesReadTag succsesFunc, ErrorReadTag errorFunc, ref int rowIndex);
        void ConwertingRead(List<TagRulsStruct> tags, string content, SuccsesReadTag succsesFunc, ErrorReadTag errorFunc, ref int rowIndex);
    }
    public class RowReader : IConverting
    {

        private bool CheckTag(string row, string tagName, out string rowContent)
        {
            rowContent = null;
            string tag = FormatTag(tagName);
            if (row == tag)
            {
                return true;
            }
            int len = tag.Length;
            if (row.Length > len && row.Remove(len) == tag)
            {
                rowContent = row.Substring(tag.Length);
                return true;
            }
            return false;
        }

        public string[] StrToRows(string content)
        {
            return content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
        public string RowsToStr(string[] rows)
        {
            return string.Join("\r\n", rows);
        }

        public string FormatTag(string tagName)//root
        {
            string bef = "<";
            string aft = ">";
            return bef + tagName + aft;
        }
        public string FormatTag(string tagName, string rowContent)
        {
            return FormatTag(tagName) + rowContent;
        }

        public void ConwertingRead(List<TagRulsStruct> tagsa, string[] rows, SuccsesReadTag succsesFunc, ErrorReadTag errorFunc, ref int rowIndex)
        {
            if (tagsa == null)
            {
                return;
            }

            List<TagRulsStruct> tags = tagsa.CloneAll();

            string rowContent = null;
            int indexLastTrue = -1;
            while (rowIndex < rows.Length)
            {
                bool findTag = false;
                foreach (TagRulsStruct tag in tags)
                {
                    if (CheckTag(rows[rowIndex], tag.TagName, out rowContent))
                    {
                        findTag = true;
                        indexLastTrue = rowIndex;
                        rowIndex++;
                        //Должен проверить, всех детей
                        succsesFunc(tag, rows[indexLastTrue], rowContent);
                        if (tag.Type == TagRuls.Один_Обязательный || tag.Type == TagRuls.Один_Необязательный)
                        {
                            tags.Remove(tag);
                        }
                        if (tag.Type == TagRuls.Закрывашка)
                        {
                            tags.Remove(tag);
                            goto End_Check;
                        }
                        ConwertingRead(tag.Children, rows, succsesFunc, errorFunc, ref rowIndex);
                        break;
                    }
                }
                if (tags.Count == 0)
                {
                    return;
                }

                if (!findTag)
                {
                    rowIndex++;
                }
            }
        End_Check:
            //Проверка оставшихся тегов
            foreach (TagRulsStruct tag in tags)
            {
                if (tag.Type == TagRuls.Один_Обязательный || tag.Type == TagRuls.Закрывашка)
                {
                    if (indexLastTrue != -1)
                    {
                        errorFunc(tag, rows[indexLastTrue], rowContent, "Обязательный тег небыл реализован", indexLastTrue);
                    }
                    else
                    {
                        errorFunc(tag, null, rowContent, "Тег не найден вовсем!", 0);
                    }

                    return;
                    //throw new Exception("Обязательный тег небыл реализован");
                }
            }

        }
        public void ConwertingRead(List<TagRulsStruct> tags, string content, SuccsesReadTag succsesFunc, ErrorReadTag errorFunc, ref int rowIndex)
        {
            ConwertingRead(tags, StrToRows(content), succsesFunc, errorFunc, ref rowIndex);
        }

    }
}
