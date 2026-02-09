using System;
using System.Collections.Generic;

namespace Roguelike.Items
{
    public static class ItemDataParseUtil
    {
        public static EItemRarity ParseRarity(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return EItemRarity.Unknown;
            return Enum.TryParse<EItemRarity>(raw.Trim(), true, out var v) ? v : EItemRarity.Unknown;
        }

        public static EConditionExpr ParseConditionExpr(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return EConditionExpr.None;
            return Enum.TryParse<EConditionExpr>(raw.Trim(), true, out var v) ? v : EConditionExpr.None;
        }

        public static string ExtractConditionTypeArg(string expr)
        {
            string type = "";
            string arg = "";

            if(string.IsNullOrWhiteSpace(expr)) return "";

            expr = expr.Trim();
            int p = expr.IndexOf('(');
            if (p < 0) return "";

            type = expr.Substring(0, p).Trim();

            int q = expr.LastIndexOf(")");
            if(q > p + 1)
            {
                arg = expr.Substring(p + 1, q - p - 1).Trim();
            }
            return arg;
        }

        public static EDamageSourceType ParseDamageSourceType(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return EDamageSourceType.None;
            return Enum.TryParse<EDamageSourceType>(raw.Trim(), true, out var v) ? v : EDamageSourceType.None;
        }

        public static string NormalizeTriggerTag(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            raw = raw.Trim();

            if (string.Equals(raw, "WaveStart", StringComparison.OrdinalIgnoreCase))
                return "OnWaveStart";

            return raw;
        }

        public static EDamagePhase ResolvePhase(string triggerTag)
        {
            if (string.IsNullOrWhiteSpace(triggerTag)) return EDamagePhase.None;
            triggerTag = triggerTag.Trim();

            if (string.Equals(triggerTag, "OnDotTick", StringComparison.OrdinalIgnoreCase)) return EDamagePhase.Tick;
            if (string.Equals(triggerTag, "OnMarkExpire", StringComparison.OrdinalIgnoreCase)) return EDamagePhase.Expire;

            if (string.Equals(triggerTag, "OnHit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(triggerTag, "MeleeHit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(triggerTag, "RangedHit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(triggerTag, "CritHit", StringComparison.OrdinalIgnoreCase))
                return EDamagePhase.Direct;

            return EDamagePhase.Proc;
        }

        public static float[] ParseLevelArray(string raw, int maxLevel)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return Array.Empty<float>();

            raw = raw.Trim();
            List<float> values = null;

            if (raw.StartsWith("[") && raw.EndsWith("]"))
            {
                var inner = raw.Substring(1, raw.Length - 2);
                values = ParseCsvLike(inner);
            }
            else if (raw.Contains("|"))
            {
                values = ParsePipe(raw);
            }
            else
            {
                if (float.TryParse(raw, out var single))
                    values = new List<float> { single };
            }

            if (values == null || values.Count == 0)
                return Array.Empty<float>();

            return Normalize(values, Math.Max(1, maxLevel));
        }

        private static List<float> ParsePipe(string s)
        {
            var list = new List<float>();
            var parts = s.Split('|');
            foreach (var p in parts)
                if (float.TryParse(p.Trim(), out var v)) list.Add(v);
            return list;
        }

        private static List<float> ParseCsvLike(string s)
        {
            var list = new List<float>();
            var parts = s.Split(',');
            foreach (var p in parts)
                if (float.TryParse(p.Trim(), out var v)) list.Add(v);
            return list;
        }

        private static float[] Normalize(List<float> values, int maxLevel)
        {
            if (values.Count == 1)
            {
                var arr = new float[maxLevel];
                for (int i = 0; i < maxLevel; i++) arr[i] = values[0];
                return arr;
            }

            if (values.Count < maxLevel)
            {
                var last = values[^1];
                while (values.Count < maxLevel) values.Add(last);
            }

            if (values.Count > maxLevel)
                values.RemoveRange(maxLevel, values.Count - maxLevel);

            return values.ToArray();
        }
    }
}
