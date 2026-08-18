using System.Collections.Generic;
using System.Text;

namespace CaravanSecrets.Features.Gameplay
{
    public static class ArabicText
    {
        private readonly struct Forms
        {
            public readonly char Isolated, Final, Initial, Medial;
            public readonly bool JoinsLeft;
            public Forms(char isolated, char final, char initial = '\0', char medial = '\0')
            { Isolated = isolated; Final = final; Initial = initial; Medial = medial; JoinsLeft = initial != '\0'; }
        }

        private static readonly Dictionary<char, Forms> Map = new()
        {
            ['\u0621']=new('\uFE80','\uFE80'), ['\u0622']=new('\uFE81','\uFE82'), ['\u0623']=new('\uFE83','\uFE84'), ['\u0625']=new('\uFE87','\uFE88'),
            ['\u0627']=new('\uFE8D','\uFE8E'), ['\u0628']=new('\uFE8F','\uFE90','\uFE91','\uFE92'), ['\u0629']=new('\uFE93','\uFE94'),
            ['\u062A']=new('\uFE95','\uFE96','\uFE97','\uFE98'), ['\u062B']=new('\uFE99','\uFE9A','\uFE9B','\uFE9C'), ['\u062C']=new('\uFE9D','\uFE9E','\uFE9F','\uFEA0'),
            ['\u062D']=new('\uFEA1','\uFEA2','\uFEA3','\uFEA4'), ['\u062E']=new('\uFEA5','\uFEA6','\uFEA7','\uFEA8'), ['\u062F']=new('\uFEA9','\uFEAA'),
            ['\u0630']=new('\uFEAB','\uFEAC'), ['\u0631']=new('\uFEAD','\uFEAE'), ['\u0632']=new('\uFEAF','\uFEB0'), ['\u0633']=new('\uFEB1','\uFEB2','\uFEB3','\uFEB4'),
            ['\u0634']=new('\uFEB5','\uFEB6','\uFEB7','\uFEB8'), ['\u0635']=new('\uFEB9','\uFEBA','\uFEBB','\uFEBC'), ['\u0636']=new('\uFEBD','\uFEBE','\uFEBF','\uFEC0'),
            ['\u0637']=new('\uFEC1','\uFEC2','\uFEC3','\uFEC4'), ['\u0638']=new('\uFEC5','\uFEC6','\uFEC7','\uFEC8'), ['\u0639']=new('\uFEC9','\uFECA','\uFECB','\uFECC'),
            ['\u063A']=new('\uFECD','\uFECE','\uFECF','\uFED0'), ['\u0641']=new('\uFED1','\uFED2','\uFED3','\uFED4'), ['\u0642']=new('\uFED5','\uFED6','\uFED7','\uFED8'),
            ['\u0643']=new('\uFED9','\uFEDA','\uFEDB','\uFEDC'), ['\u0644']=new('\uFEDD','\uFEDE','\uFEDF','\uFEE0'), ['\u0645']=new('\uFEE1','\uFEE2','\uFEE3','\uFEE4'),
            ['\u0646']=new('\uFEE5','\uFEE6','\uFEE7','\uFEE8'), ['\u0647']=new('\uFEE9','\uFEEA','\uFEEB','\uFEEC'), ['\u0648']=new('\uFEED','\uFEEE'),
            ['\u0649']=new('\uFEEF','\uFEF0'), ['\u064A']=new('\uFEF1','\uFEF2','\uFEF3','\uFEF4'), ['\u0626']=new('\uFE89','\uFE8A','\uFE8B','\uFE8C'), ['\u0624']=new('\uFE85','\uFE86')
        };

        public static string Display(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var lines = text.Replace("\r", string.Empty).Split('\n');
            for (var i = 0; i < lines.Length; i++) lines[i] = ShapeLine(lines[i]);
            return string.Join("\n", lines);
        }

        private static string ShapeLine(string line)
        {
            var shaped = new char[line.Length];
            for (var i = 0; i < line.Length; i++)
            {
                if (!Map.TryGetValue(line[i], out var forms)) { shaped[i] = line[i]; continue; }
                var joinsPrevious = i > 0 && Map.TryGetValue(line[i - 1], out var previous) && previous.JoinsLeft;
                var joinsNext = forms.JoinsLeft && i + 1 < line.Length && Map.ContainsKey(line[i + 1]);
                shaped[i] = joinsPrevious && joinsNext ? forms.Medial : joinsPrevious ? forms.Final : joinsNext ? forms.Initial : forms.Isolated;
            }

            return ReversePreservingProtectedRuns(shaped);
        }

        private static string ReversePreservingProtectedRuns(char[] shaped)
        {
            var parts = new List<(bool protect, string text)>();
            var i = 0;
            while (i < shaped.Length)
            {
                var protect = IsProtected(shaped[i]);
                var start = i;
                i++;
                while (i < shaped.Length && IsProtected(shaped[i]) == protect) i++;
                parts.Add((protect, new string(shaped, start, i - start)));
            }

            parts.Reverse();
            var result = new StringBuilder(shaped.Length);
            foreach (var part in parts)
            {
                if (part.protect) result.Append(part.text);
                else
                {
                    for (var j = part.text.Length - 1; j >= 0; j--) result.Append(part.text[j]);
                }
            }
            return result.ToString();
        }

        private static bool IsProtected(char c) =>
            char.IsDigit(c) || c is '/' or ':' or '.' or '%' or '+' or '-' or '#' ||
            c is >= 'A' and <= 'Z' || c is >= 'a' and <= 'z';
    }
}
