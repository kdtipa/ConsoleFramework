using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public static class CharHelper
{
    public static char Ellipse { get; } = '…';

    public static char LongHyphen { get; } = '–';

    public static char OpenDoubleQuote { get; } = '“';

    public static char CloseDoubleQuote { get; } = '”';

    public static char OpenSingleQuote { get; } = '‘';

    public static char CloseSingleQuote { get; } = '’';

    public static char BulletPoint { get; } = '•';

    public static char CurrencyDollar { get; } = '$';

    public static char CurrencyCent { get; } = '¢';

    public static char CurrencyPound { get; } = '£';

    public static char CurrencyYen { get; } = '¥';

    /// <summary>
    /// Cleans the string to remove the fancy versions of some characters.  
    /// Specifically, it changes the single character ellipse to three period 
    /// characters; the long hyphen to the normal one; and the fancy open and 
    /// close quotes (single or double) to the normal ones.
    /// </summary>
    public static string UseBasicCharacters(this string sourceString)
    {
        var result = new StringBuilder();

        foreach (char c in sourceString)
        {
            switch (c)
            {
                case '…': result.Append("..."); break;
                case '–': result.Append('-'); break;
                case '“': result.Append('"'); break;
                case '”': result.Append('"'); break;
                case '‘': result.Append('\''); break;
                case '’': result.Append('\''); break;
                default: result.Append(c); break;
            }
        }

        return result.ToString();
    }


    public static char[] WordBreakCharacters { get; } = [' ', '.', ',', '?', '!', '(', ')', '+', '=', '{', '}', '[', ']', '|', '"', ';', '<', '>', (char)9, (char)10, (char)13];

    public static char[] SentenceBreakCharacters { get; } = ['.', '?', '!'];

    public static char[] WhiteSpaceCharacters { get; } = [' ', (char)9, (char)10, (char)11, (char)12, (char)13];

    /// <summary>
    /// This is an extension method that makes use of the built-in char.IsWhiteSpace() 
    /// method, but if the character is not considered white space, we check our own 
    /// WhiteSpaceCharacters property too just to be sure.
    /// </summary>
    /// <returns>true if the character counts as white space</returns>
    public static bool IsWhiteSpaceCharacter(this char c)
    {
        return char.IsWhiteSpace(c) || WhiteSpaceCharacters.Contains(c);
    }

    public static char[] LineBreakCharacters { get; } = ['\n', (char)10, (char)11, (char)12, (char)13];

    public static bool IsLineBreakCharacter(this char c)
    {
        return LineBreakCharacters.Contains(c);
    }

    public static char ToUpper(this char c)
    {
        // basic ASCII characters...
        if (c >= 97 && c <= 122)
        {
            return (char)(c - 32);
        }

        // expanded list of characters...
        if (CharactersWithUpperAndLowerCase.Any(culc => culc == c))
        {
            return CharactersWithUpperAndLowerCase.Where(culc => culc == c).First().Upper;
        }

        // doesn't have upper case...
        return c;
    }

    public static bool IsUpper(this char c)
    {
        return (c >= 65 && c <= 90) || CharactersWithUpperAndLowerCase.Select(culc => culc.Upper).Any(u => u == c);
    }

    public static char ToLower(this char c)
    {
        // basic ASCII characters...
        if (c >= 65 && c <= 90)
        {
            return (char)(c + 32);
        }

        // expanded list of characters..
        if (CharactersWithUpperAndLowerCase.Any(culc => culc == c))
        {
            return CharactersWithUpperAndLowerCase.Where(culc => culc == c).First().Lower;
        }

        return c;
    }

    public static bool IsLower(this char c)
    {
        return (c >= 97 && c <= 122) || CharactersWithUpperAndLowerCase.Select(culc => culc.Lower).Any(l => l == c);
    }

    public static char ToggleCase(this char c)
    {
        // functionality of the case tests might be odd for non-letters, so include the third case.
        if (c.IsUpper()) { return c.ToLower(); }
        else if (c.IsLower()) { return c.ToUpper(); }
        return c;
    }

    public static bool IsNumberDigit(this char c) { return c >= 48 && c <= 57; }

    public static int? GetNumberDigitValue(this char c)
    {
        if (c.IsNumberDigit()) { return c - 48; }
        return null;
    }

    /// <summary>
    /// Has to be a string return value because of the ellipse where '…' becomes "...", but 
    /// other than that just cleans up those obnoxious quote types and long dashes.
    /// </summary>
    public static string GetBasicCharacter(this char c)
    {
        if (c == Ellipse) { return "..."; }

        if (c == LongHyphen) { return "-"; }

        if (c == OpenDoubleQuote || c == CloseDoubleQuote) { return "\""; }

        if (c == OpenSingleQuote || c == CloseSingleQuote) { return "'"; }

        return c.ToString();
    }

    public static int CountOfCharacters(string haystackString, params char[] characters)
    {
        // handle the parameters
        if (characters is null || characters.Length == 0) { return 0; }
        if (string.IsNullOrEmpty(haystackString)) { return 0; }

        // see if the characters exist in the string and how many times in total.
        int count = 0;
        foreach (char c in haystackString)
        {
            if (characters.Contains(c)) { count++; }
        }

        // return the count
        return count;
    }

    /// <summary>
    /// This dictionary gives you a way to look through the ASCII, Extended Characters, and Alt-Code Characters as defined 
    /// in the Windows environment.  Numbers 0 to 127 are the standard ASCII characters (with string descriptions for the 
    /// control characters).  128 to 255 are the characters as though you typed alt-N.  1280 to 2550 (going up by 10s) are 
    /// the characters as though you typed alt-N with the preceeding zero, like alt-0128.  For purpose of an integer key, 
    /// we just moved the zero to the ones place.
    /// </summary>
    public static Dictionary<int, string> CharacterCodeToChar { get; } = new()
    {
        // ASCII (0 to 127)
        { 0, "[Null]" }, { 1, "[Heading Start]" }, { 2, "[Text Start]" }, { 3, "[Text End]" }, { 4, "[Transmission End]" },
        { 5, "[Enquiry]" }, { 6, "[Acknowledge]" }, { 7, "[Bell]" }, { 8, "[Backspace]" }, { 9, "[Horizontal Tab]"},
        { 10, "[Line Feed]" }, { 11, "[Vertical Tab]" }, { 12, "[Form Feed]" }, { 13, "[Carriage Return]" }, { 14, "[Shift Out]" },
        { 15, "[Shift In]" }, { 16, "[Data Link Escape]" }, { 17, "[Device Control 1]" }, { 18, "[Device Control 2]" }, { 19, "[Device Control 3]" }, 
        { 20, "[Device Control 4]" }, { 21, "[Negative Acknowledge]" }, { 22, "[Synchronous Idle]" }, { 23, "[Transmission Block End]" }, { 24, "[Cancel]" },
        { 25, "[End of Medium]" }, { 26, "[Substitute]" }, { 27, "[Escape]" }, { 28, "[File Separator]" }, { 29, "[Group Separator]" },
        { 30, "[Record Separator]" }, { 31, "[Unit Separator]" }, { 32, "[Space]" },
        { 33, "!" }, { 34, "\"" }, { 35, "#" }, { 36, "$" }, { 37, "%" }, { 38, "&" }, { 39, "'" }, 
        { 40, "(" }, { 41, ")" }, { 42, "*" }, { 43, "," }, { 45, "-" }, { 46, "." }, { 47, "/" },
        { 48, "0" }, { 49, "1" }, { 50, "2" }, { 51, "3" }, { 52, "4" }, 
        { 53, "5" }, { 54, "6" }, { 55, "7" }, { 56, "8" }, { 57, "9" }, 
        { 58, ":" }, { 59, ";" }, { 60, "<" }, { 61, "=" }, { 62, ">" }, { 63, "?" }, { 64, "@" }, 
        { 65, "A" }, { 66, "B" }, { 67, "C" }, { 68, "D" }, { 69, "E" }, { 70, "F" }, 
        { 71, "G" }, { 72, "H" }, { 73, "I" }, { 74, "J" }, { 75, "K" }, { 76, "L" }, { 77, "M" }, 
        { 78, "N" }, { 79, "O" }, { 80, "P" }, { 81, "Q" }, { 82, "R" }, { 83, "S" }, 
        { 84, "T" }, { 85, "U" }, { 86, "V" }, { 87, "W" }, { 88, "X" }, { 89, "Y" }, { 90, "Z" }, 
        { 91, "[" }, { 92, "\\" }, { 93, "]" }, { 94, "^" }, { 95, "_" }, { 96, "`" }, 
        { 97, "a" }, { 98, "b" }, { 99, "c" }, { 100, "d" }, { 101, "e" }, { 102, "f" }, 
        { 103, "g" }, { 104, "h" }, { 105, "i" }, { 106, "j" }, { 107, "k" }, { 108, "l" }, { 109, "m" }, 
        { 110, "n" }, { 111, "o" }, { 112, "p" }, { 113, "q" }, { 114, "r" }, { 115, "s" }, 
        { 116, "t" }, { 117, "u" }, { 118, "v" }, { 119, "w" }, { 120, "x" }, { 121, "y" }, { 122, "z" }, 
        { 123, "{" }, { 124, "|" }, { 125, "}" }, { 126, "~" }, { 127, "[Delete]" }, 
        // Windows Character Codes without preceeding zero (alt-128 to alt-255)
        { 128, "Ç" }, { 129, "ü" }, { 130, "é" }, { 131, "â" }, { 132, "ä" }, { 133, "à" }, { 134, "å" }, { 135, "ç" }, { 136, "ê" }, { 137, "ë" },
        { 138, "è" }, { 139, "ï" }, { 140, "î" }, { 141, "ì" }, { 142, "Ä" }, { 143, "Å" }, { 144, "É" }, { 145, "æ" }, { 146, "Æ" }, { 147, "ô" }, 
        { 148, "ö" }, { 149, "ò" }, { 150, "û" }, { 151, "ù" }, { 152, "ÿ" }, { 153, "Ö" }, { 154, "Ü" }, { 155, "¢" }, { 156, "£" }, { 157, "¥" }, 
        { 158, "₧" }, { 159, "ƒ" }, { 160, "á" }, { 161, "í" }, { 162, "ó" }, { 163, "ú" }, { 164, "ñ" }, { 165, "Ñ" }, { 166, "ª" }, { 167, "º" }, 
        { 168, "¿" }, { 169, "⌐" }, { 170, "¬" }, { 171, "½" }, { 172, "¼" }, { 173, "¡" }, { 174, "«" }, { 175, "»" }, { 176, "░" }, { 177, "▒" }, 
        { 178, "▓" }, { 179, "│" }, { 180, "┤" }, { 181, "╡" }, { 182, "╢" }, { 183, "╖" }, { 184, "╕" }, { 185, "╣" }, { 186, "║" }, { 187, "╗" }, 
        { 188, "╝" }, { 189, "╜" }, { 190, "╛" }, { 191, "┐" }, { 192, "└" }, { 193, "┴" }, { 194, "┬" }, { 195, "├" }, { 196, "─" }, { 197, "┼" }, 
        { 198, "╞" }, { 199, "╟" }, { 200, "╚" }, { 201, "╔" }, { 202, "╩" }, { 203, "╦" }, { 204, "╠" }, { 205, "═" }, { 206, "╬" }, { 207, "╧" }, 
        { 208, "╨" }, { 209, "╤" }, { 210, "╥" }, { 211, "╙" }, { 212, "╘" }, { 213, "╒" }, { 214, "╓" }, { 215, "╫" }, { 216, "╪" }, { 217, "┘" }, 
        { 218, "┌" }, { 219, "█" }, { 220, "▄" }, { 221, "▌" }, { 222, "▐" }, { 223, "▀" }, { 224, "α" }, { 225, "ß" }, { 226, "Γ" }, { 227, "π" }, 
        { 228, "Σ" }, { 229, "σ" }, { 230, "µ" }, { 231, "τ" }, { 232, "Φ" }, { 233, "Θ" }, { 234, "Ω" }, { 235, "δ" }, { 236, "∞" }, { 237, "φ" }, 
        { 238, "ε" }, { 239, "∩" }, { 240, "≡" }, { 241, "±" }, { 242, "≥" }, { 243, "≤" }, { 244, "⌠" }, { 245, "⌡" }, { 246, "÷" }, { 247, "≈" }, 
        { 248, "°" }, { 249, "∙" }, { 250, "·" }, { 251, "√" }, { 252, "ⁿ" }, { 253, "²" }, { 254, "■" }, { 255, "[Non-Breaking Space]" },
        // Windows Character Codes WITH preceeding zero, but for our
        // dictionary essentially put the zero in the ones place
        // (alt-0128 to alt-0255 valued at 1280 to 2550)
        { 1280, "€" }, { 1290, "[Ctrl Char Delete]"}, { 1300, "‚" }, { 1310, "ƒ" }, { 1320, "„" }, { 1330, "…" }, { 1340, "†" }, { 1350, "‡" }, { 1360, "ˆ" }, { 1370, "‰" },
        { 1380, "Š" }, { 1390, "‹"}, { 1400, "Œ" }, { 1410, "[Reverse Line Feed]" }, { 1420, "Ž" }, { 1430, "[Single Shift Three]" }, { 1440, "[Device Control String]" }, { 1450, "‘" }, { 1460, "’" }, { 1470, "”" },
        { 1480, "”" }, { 1490, "•"}, { 1500, "–" }, { 1510, "—" }, { 1520, "˜" }, { 1530, "™" }, { 1540, "š" }, { 1550, "›" }, { 1560, "œ" }, { 1570, "[Operating System Command]" },
        { 1580, "ž" }, { 1590, "Ÿ"}, { 1600, "[Non-Breaking Space]" }, { 1610, "¡" }, { 1620, "¢" }, { 1630, "£" }, { 1640, "¤" }, { 1650, "¥" }, { 1660, "¦" }, { 1670, "§" },
        { 1680, "¨" }, { 1690, "©"}, { 1700, "ª" }, { 1710, "«" }, { 1720, "¬" }, { 1730, "[Soft Hyphen]" }, { 1740, "®" }, { 1750, "¯" }, { 1760, "°" }, { 1770, "±" },
        { 1780, "²" }, { 1790, "³"}, { 1800, "´" }, { 1810, "µ" }, { 1820, "¶" }, { 1830, "·" }, { 1840, "¸" }, { 1850, "¹" }, { 1860, "º" }, { 1870, "»" },
        { 1880, "¼" }, { 1890, "½"}, { 1900, "¾" }, { 1910, "¿" }, { 1920, "À" }, { 1930, "Á" }, { 1940, "Â" }, { 1950, "Ã" }, { 1960, "Ä" }, { 1970, "Å" },
        { 1980, "Æ" }, { 1990, "Ç"}, { 2000, "È" }, { 2010, "É" }, { 2020, "Ê" }, { 2030, "Ë" }, { 2040, "Ì" }, { 2050, "Í" }, { 2060, "Î" }, { 2070, "Ï" },
        { 2080, "Ð" }, { 2090, "Ñ"}, { 2100, "Ò" }, { 2110, "Ó" }, { 2120, "Ô" }, { 2130, "Õ" }, { 2140, "Ö" }, { 2150, "×" }, { 2160, "Ø" }, { 2170, "Ù" },
        { 2180, "Ú" }, { 2190, "Û"}, { 2200, "Ü" }, { 2210, "Ý" }, { 2220, "Þ" }, { 2230, "ß" }, { 2240, "à" }, { 2250, "á" }, { 2260, "â" }, { 2270, "ã" },
        { 2280, "ä" }, { 2290, "å"}, { 2300, "æ" }, { 2310, "ç" }, { 2320, "è" }, { 2330, "é" }, { 2340, "ê" }, { 2350, "ë" }, { 2360, "ì" }, { 2370, "í" },
        { 2380, "î" }, { 2390, "ï"}, { 2400, "ð" }, { 2410, "ñ" }, { 2420, "ò" }, { 2430, "ó" }, { 2440, "ô" }, { 2450, "õ" }, { 2460, "ö" }, { 2470, "÷" },
        { 2480, "ø" }, { 2490, "ù"}, { 2500, "ú" }, { 2510, "û" }, { 2520, "ü" }, { 2530, "ý" }, { 2540, "þ" }, { 2550, "ÿ" }
    };


    public static char[] PrintableCharacters { get; } =
    {
        '\b', '\t', (char)10, (char)11, (char)12, (char)13, // line feed, vertical tab, form feed, carriage return
        ' ', '!', '\"', '#', '$', '%', '&', '\'',
        '(', ')', '*', ',', '-', '.', '/',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        ':', ';', '<', '=', '>', '?', '@',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '[', '\\', ']', '^', '_', '`', 
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        '{', '|', '}', '~', 
        // Windows Character Codes without preceeding zero (alt-128 to alt-255)
        'Ç', 'ü', 'é', 'â', 'ä', 'à', 'å', 'ç', 'ê', 'ë',
        'è', 'ï', 'î', 'ì', 'Ä', 'Å', 'É', 'æ', 'Æ', 'ô',
        'ö', 'ò', 'û', 'ù', 'ÿ', 'Ö', 'Ü', '¢', '£', '¥',
        '₧', 'ƒ', 'á', 'í', 'ó', 'ú', 'ñ', 'Ñ', 'ª', 'º',
        '¿', '⌐', '¬', '½', '¼', '¡', '«', '»', '░', '▒',
        '▓', '│', '┤', '╡', '╢', '╖', '╕', '╣', '║', '╗',
        '╝', '╜', '╛', '┐', '└', '┴', '┬', '├', '─', '┼',
        '╞', '╟', '╚', '╔', '╩', '╦', '╠', '═', '╬', '╧',
        '╨', '╤', '╥', '╙', '╘', '╒', '╓', '╫', '╪', '┘',
        '┌', '█', '▄', '▌', '▐', '▀', 'α', 'ß', 'Γ', 'π',
        'Σ', 'σ', 'µ', 'τ', 'Φ', 'Θ', 'Ω', 'δ', '∞', 'φ',
        'ε', '∩', '≡', '±', '≥', '≤', '⌠', '⌡', '÷', '≈',
        '°', '∙', '·', '√', 'ⁿ', '²', '■', ' ', // that space is a non-breaking space
        // Windows Character Codes WITH preceeding zero, but for our
        // dictionary (above) essentially put the zero in the ones place
        // (alt-0128 to alt-0255 valued at 1280 to 2550)
        '€', '‚', 'ƒ', '„', '…', '†', '‡', 'ˆ', '‰',
        'Š', '‹', 'Œ', 'Ž', '‘', '’', '”',
        '”', '•', '–', '—', '˜', '™', 'š', '›', 'œ',
        'ž', 'Ÿ', ' ', '¡', '¢', '£', '¤', '¥', '¦', '§', // the space on this line is non-breaking
        '¨', '©', 'ª', '«', '¬', '­', '®', '¯', '°', '±', // the apparently empty quotes on this line are a "soft hyphen"
        '²', '³', '´', 'µ', '¶', '·', '¸', '¹', 'º', '»',
        '¼', '½', '¾', '¿', 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å',
        'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï',
        'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', '×', 'Ø', 'Ù',
        'Ú', 'Û', 'Ü', 'Ý', 'Þ', 'ß', 'à', 'á', 'â', 'ã',
        'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í',
        'î', 'ï', 'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', '÷',
        'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'þ', 'ÿ'
    };

    public static List<CaseChar> CharactersWithUpperAndLowerCase { get; } = new List<CaseChar>()
    {
        new CaseChar('a', 'A'), new CaseChar('b', 'B'), new CaseChar('c', 'C'), new CaseChar('d', 'D'), new CaseChar('e', 'E'), 
        new CaseChar('f', 'F'), new CaseChar('g', 'G'), new CaseChar('h', 'H'), new CaseChar('i', 'I'), new CaseChar('j', 'J'), 
        new CaseChar('k', 'K'), new CaseChar('l', 'L'), new CaseChar('m', 'M'), new CaseChar('n', 'N'), new CaseChar('o', 'O'),
        new CaseChar('p', 'P'), new CaseChar('q', 'Q'), new CaseChar('r', 'R'), new CaseChar('s', 'S'), new CaseChar('t', 'T'),
        new CaseChar('u', 'U'), new CaseChar('v', 'V'), new CaseChar('w', 'W'), new CaseChar('x', 'X'), new CaseChar('y', 'Y'),
        new CaseChar('z', 'Z'), 
        // seems like duplicates but are different alt-codes... which may under-the-hood be the same, but we're being cautious/paranoid
        new CaseChar('ä', 'Ä'), new CaseChar('ä', 'Ä'), 
        new CaseChar('å', 'Å'), new CaseChar('å', 'Å'),
        new CaseChar('æ', 'Æ'), new CaseChar('æ', 'Æ'),
        new CaseChar('ç', 'Ç'), new CaseChar('ç', 'Ç'), 
        // here we have things that seem to be unique...
        new CaseChar('à', 'À'), new CaseChar('á', 'Á'), new CaseChar('â', 'Â'), new CaseChar('ã', 'Ã'),
        new CaseChar('è', 'È'), new CaseChar('é', 'É'), new CaseChar('ê', 'Ê'), new CaseChar('ë', 'Ë'),
        new CaseChar('ì', 'Ì'), new CaseChar('í', 'Í'), new CaseChar('î', 'Î'), new CaseChar('ï', 'Ï'),
        new CaseChar('ò', 'Ò'), new CaseChar('ó', 'Ó'), new CaseChar('ô', 'Ô'), new CaseChar('õ', 'Õ'), new CaseChar('ö', 'Ö'),
        new CaseChar('ð', 'Ð'), new CaseChar('ñ', 'Ñ'), new CaseChar('þ', 'Þ'), new CaseChar('ø', 'Ø'), 
        new CaseChar('ÿ', 'Ÿ'), new CaseChar('š', 'Š'), 
        new CaseChar('œ', 'Œ'), new CaseChar('ž', 'Ž')
    };

    public static bool CharacterEqual(this char c, char compare, bool ignoreCase)
    {
        if (ignoreCase) { return c.ToUpper() == compare.ToUpper(); }
        return c == compare;
    }

    public static Dictionary<char, char> Umlaut { get; } = new()
    {
        { 'A', 'Ä' }, { 'a', 'ä' }, 
        { 'E', 'Ë' }, { 'e', 'ë' },
        { 'I', 'Ï' }, { 'i', 'ï' },
        { 'O', 'Ö' }, { 'o', 'ö' },
        { 'U', 'Ü' }, { 'u', 'ü' }, 
        { 'y', 'ÿ' }
    };

    /// <summary>
    /// If the character is one of the vowels that has a character with 
    /// an umlaut, you'll get back the character with an umlaut.  If not, 
    /// you'll get back the original character.
    /// </summary>
    public static char AddUmlaut(this char srcChar)
    {
        if (Umlaut.TryGetValue(srcChar, out var result)) { return result; }
        return srcChar;
    }

    /// <summary>
    /// Uses the Umlaut dictionary to see if the character you 
    /// pass in has an umlaut version and returns that if true.  
    /// Otherwise, it just returns the character you sent in.
    /// </summary>
    public static char GetUmlaut(char srcChar)
    {
        if (Umlaut.TryGetValue(srcChar, out char result))
        {
            return result;
        }
        return srcChar;
    }

    public static char DegreeSign { get; } = '°';

    public static char InfinitySign { get; } = '∞';

    public static char PiSign { get; } = 'π';

    public static char EszettSign { get; } = 'ß';

    


    public static string CharArrayToString(params char[] sourceArray)
    {
        var result = new StringBuilder();
        foreach (char c in sourceArray) { result.Append(c); }
        return result.ToString();
    }

    public static IEnumerable<string> CharMatrixToString(char[,] sourceMatrix)
    {
        var line = new StringBuilder();
        var cols = sourceMatrix.GetLength(0);
        var rows = sourceMatrix.GetLength(1);


        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                char current = sourceMatrix[c, r];
                if (current == (char)10 || current == (char)13 || current == '\n')
                {
                    yield return line.ToString();
                    line.Clear();
                }
                else
                {
                    line.Append(current);
                }
            }

            if (line.Length > 0) { yield return line.ToString(); line.Clear(); }
        }
    }


}


public struct CaseChar : IEquatable<CaseChar>
{
    public CaseChar(char lower, char upper)
    {
        Lower = lower;
        Upper = upper;
    }

    public char Lower { get; }

    public char Upper { get; }

    public bool Equals(CaseChar other)
    {
        return Lower == other.Lower && Upper == other.Upper;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is CaseChar ccObj) { return Equals(ccObj); }

        if (obj is char cObj) { return Lower == cObj || Upper == cObj; }

        return false;
    }

    public override int GetHashCode()
    {
        return Lower.GetHashCode();
    }

    public override string ToString()
    {
        return Lower.ToString();
    }

    public static bool operator ==(CaseChar a, CaseChar b) { return a.Equals(b); }
    public static bool operator !=(CaseChar a, CaseChar b) { return !a.Equals(b); }

    public static bool operator ==(CaseChar a, char b) { return a.Equals(b); }
    public static bool operator !=(CaseChar a, char b) { return !a.Equals(b); }

    public static bool operator ==(char a, CaseChar b) { return b.Equals(a); }
    public static bool operator !=(char a, CaseChar b) { return !b.Equals(a); }


}
