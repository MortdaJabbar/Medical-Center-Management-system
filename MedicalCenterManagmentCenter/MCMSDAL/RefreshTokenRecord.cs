using System.Globalization;
using System.Text;

namespace MCMSDAL
{
    public class RefreshTokenRecord
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedByIp { get; set; }
        public string? UserAgent { get; set; }
    }

    public static class RefreshTokenCsvData
    {
        private static readonly SemaphoreSlim _lock = new(1, 1);

        private const string Header =
            "TokenId,UserId,TokenHash,ExpiresAtUtc,RevokedAtUtc,ReplacedByTokenHash,CreatedAtUtc,CreatedByIp,UserAgent";

        public static async Task EnsureFileAsync(string csvPath)
        {
            var dir = Path.GetDirectoryName(csvPath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(csvPath))
                await File.WriteAllTextAsync(csvPath, Header + Environment.NewLine, Encoding.UTF8);
        }

        public static async Task<List<RefreshTokenRecord>> GetAllAsync(string csvPath)
        {
            await EnsureFileAsync(csvPath);

            await _lock.WaitAsync();
            try
            {
                var lines = await File.ReadAllLinesAsync(csvPath, Encoding.UTF8);
                var list = new List<RefreshTokenRecord>();

                // skip header
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = SplitCsvLine(line);
                    if (cols.Count < 9) continue;

                    list.Add(new RefreshTokenRecord
                    {
                        TokenId = Guid.Parse(cols[0]),
                        UserId = Guid.Parse(cols[1]),
                        TokenHash = cols[2],
                        ExpiresAtUtc = DateTime.Parse(cols[3], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
                        RevokedAtUtc = string.IsNullOrWhiteSpace(cols[4]) ? null :
                            DateTime.Parse(cols[4], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
                        ReplacedByTokenHash = string.IsNullOrWhiteSpace(cols[5]) ? null : cols[5],
                        CreatedAtUtc = DateTime.Parse(cols[6], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
                        CreatedByIp = string.IsNullOrWhiteSpace(cols[7]) ? null : cols[7],
                        UserAgent = string.IsNullOrWhiteSpace(cols[8]) ? null : cols[8],
                    });
                }

                return list;
            }
            finally
            {
                _lock.Release();
            }
        }

        public static async Task AddAsync(string csvPath, RefreshTokenRecord rec)
        {
            await EnsureFileAsync(csvPath);

            var line = string.Join(",",
                rec.TokenId.ToString(),
                rec.UserId.ToString(),
                EscapeCsv(rec.TokenHash),
                rec.ExpiresAtUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                rec.RevokedAtUtc?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture) ?? "",
                EscapeCsv(rec.ReplacedByTokenHash ?? ""),
                rec.CreatedAtUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                EscapeCsv(rec.CreatedByIp ?? ""),
                EscapeCsv(rec.UserAgent ?? "")
            );

            await _lock.WaitAsync();
            try
            {
                await File.AppendAllTextAsync(csvPath, line + Environment.NewLine, Encoding.UTF8);
            }
            finally
            {
                _lock.Release();
            }
        }

        public static async Task<bool> UpdateAsync(string csvPath, Func<RefreshTokenRecord, bool> predicate, Action<RefreshTokenRecord> update)
        {
            await EnsureFileAsync(csvPath);

            await _lock.WaitAsync();
            try
            {
                var lines = await File.ReadAllLinesAsync(csvPath, Encoding.UTF8);
                if (lines.Length <= 1) return false;

                var header = lines[0];
                var records = new List<RefreshTokenRecord>();

                for (int i = 1; i < lines.Length; i++)
                {
                    var cols = SplitCsvLine(lines[i]);
                    if (cols.Count < 9) continue;

                    records.Add(new RefreshTokenRecord
                    {
                        TokenId = Guid.Parse(cols[0]),
                        UserId = Guid.Parse(cols[1]),
                        TokenHash = cols[2],
                        ExpiresAtUtc = DateTime.Parse(cols[3], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
                        RevokedAtUtc = string.IsNullOrWhiteSpace(cols[4]) ? null :
                            DateTime.Parse(cols[4], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
                        ReplacedByTokenHash = string.IsNullOrWhiteSpace(cols[5]) ? null : cols[5],
                        CreatedAtUtc = DateTime.Parse(cols[6], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
                        CreatedByIp = string.IsNullOrWhiteSpace(cols[7]) ? null : cols[7],
                        UserAgent = string.IsNullOrWhiteSpace(cols[8]) ? null : cols[8],
                    });
                }

                bool changed = false;
                foreach (var r in records)
                {
                    if (predicate(r))
                    {
                        update(r);
                        changed = true;
                    }
                }

                if (!changed) return false;

                var outLines = new List<string> { header };
                foreach (var r in records)
                {
                    outLines.Add(string.Join(",",
                        r.TokenId.ToString(),
                        r.UserId.ToString(),
                        EscapeCsv(r.TokenHash),
                        r.ExpiresAtUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                        r.RevokedAtUtc?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture) ?? "",
                        EscapeCsv(r.ReplacedByTokenHash ?? ""),
                        r.CreatedAtUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                        EscapeCsv(r.CreatedByIp ?? ""),
                        EscapeCsv(r.UserAgent ?? "")
                    ));
                }

                await File.WriteAllLinesAsync(csvPath, outLines, Encoding.UTF8);
                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        // -------- CSV helpers (simple + safe) --------
        private static string EscapeCsv(string value)
        {
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        private static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else sb.Append(c);
                }
                else
                {
                    if (c == ',')
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (c == '"') inQuotes = true;
                    else sb.Append(c);
                }
            }

            result.Add(sb.ToString());
            return result;
        }
    }
}