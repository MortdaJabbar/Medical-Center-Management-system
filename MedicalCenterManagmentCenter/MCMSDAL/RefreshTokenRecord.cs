using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace MCMSDAL
{
    public class RefreshTokenRecord
    {
        public string TokenHash { get; set; } = "";
        public Guid UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public string? UserAgent { get; set; }
    }

    public static class RefreshTokenFileData
    {
        private static readonly SemaphoreSlim _lock = new(1, 1);

        // Change this to your base-dir helper if you already have one
        private static string FilePath => Path.Combine(AppContext.BaseDirectory, "data", "refresh_tokens.csv");

        public static string HashToken(string rawToken)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes); // 64 hex chars
        }

        public static async Task EnsureCreatedAsync()
        {
            var dir = Path.GetDirectoryName(FilePath)!;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (!File.Exists(FilePath))
            {
                var header = "TokenHash,UserId,CreatedAtUtc,ExpiresAtUtc,RevokedAtUtc,ReplacedByTokenHash,CreatedByIp,RevokedByIp,UserAgent";
                await File.WriteAllTextAsync(FilePath, header + Environment.NewLine);
            }
        }

        public static async Task AddAsync(RefreshTokenRecord rec)
        {
            await _lock.WaitAsync();
            try
            {
                await EnsureCreatedAsync();

                // optional: prevent duplicates
                var existing = await FindByHashAsync(rec.TokenHash);
                if (existing != null) return;

                var line = string.Join(",",
                    Csv(rec.TokenHash),
                    Csv(rec.UserId.ToString()),
                    Csv(rec.CreatedAtUtc.ToString("O")),
                    Csv(rec.ExpiresAtUtc.ToString("O")),
                    Csv(rec.RevokedAtUtc?.ToString("O")),
                    Csv(rec.ReplacedByTokenHash),
                    Csv(rec.CreatedByIp),
                    Csv(rec.RevokedByIp),
                    Csv(rec.UserAgent)
                );

                await File.AppendAllTextAsync(FilePath, line + Environment.NewLine);
            }
            finally
            {
                _lock.Release();
            }
        }

        public static async Task<RefreshTokenRecord?> FindByHashAsync(string tokenHash)
        {
            await _lock.WaitAsync();
            try
            {
                await EnsureCreatedAsync();

                var lines = await File.ReadAllLinesAsync(FilePath);
                // skip header
                for (int i = 1; i < lines.Length; i++)
                {
                    var cols = ParseCsvLine(lines[i]);
                    if (cols.Count < 9) continue;

                    if (string.Equals(cols[0], tokenHash, StringComparison.OrdinalIgnoreCase))
                        return Map(cols);
                }
                return null;
            }
            finally
            {
                _lock.Release();
            }
        }

        public static async Task<List<RefreshTokenRecord>> GetByUserIdAsync(Guid userId)
        {
            await _lock.WaitAsync();
            try
            {
                await EnsureCreatedAsync();

                var result = new List<RefreshTokenRecord>();
                var lines = await File.ReadAllLinesAsync(FilePath);

                for (int i = 1; i < lines.Length; i++)
                {
                    var cols = ParseCsvLine(lines[i]);
                    if (cols.Count < 9) continue;

                    if (Guid.TryParse(cols[1], out var uid) && uid == userId)
                        result.Add(Map(cols));
                }

                return result;
            }
            finally
            {
                _lock.Release();
            }
        }

        // revoke a token (and optionally set replaced-by hash)
        public static async Task<bool> RevokeAsync(string tokenHash, DateTime revokedAtUtc, string? revokedByIp, string? replacedByTokenHash = null)
        {
            await _lock.WaitAsync();
            try
            {
                await EnsureCreatedAsync();

                var lines = (await File.ReadAllLinesAsync(FilePath)).ToList();
                bool updated = false;

                for (int i = 1; i < lines.Count; i++)
                {
                    var cols = ParseCsvLine(lines[i]);
                    if (cols.Count < 9) continue;

                    if (!string.Equals(cols[0], tokenHash, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Already revoked -> do nothing
                    if (!string.IsNullOrWhiteSpace(cols[4])) return false;

                    cols[4] = revokedAtUtc.ToString("O");
                    cols[5] = replacedByTokenHash ?? cols[5];
                    cols[7] = revokedByIp ?? cols[7];

                    lines[i] = string.Join(",", cols.Select(Csv));
                    updated = true;
                    break;
                }

                if (updated)
                    await File.WriteAllLinesAsync(FilePath, lines);

                return updated;
            }
            finally
            {
                _lock.Release();
            }
        }

        public static bool IsExpired(RefreshTokenRecord rec, DateTime nowUtc)
            => rec.ExpiresAtUtc <= nowUtc;

        public static bool IsRevoked(RefreshTokenRecord rec)
            => rec.RevokedAtUtc.HasValue;

        // ---------- helpers ----------
        private static RefreshTokenRecord Map(List<string> c)
        {
            return new RefreshTokenRecord
            {
                TokenHash = c[0],
                UserId = Guid.Parse(c[1]),
                CreatedAtUtc = DateTime.Parse(c[2], null, DateTimeStyles.RoundtripKind),
                ExpiresAtUtc = DateTime.Parse(c[3], null, DateTimeStyles.RoundtripKind),
                RevokedAtUtc = string.IsNullOrWhiteSpace(c[4]) ? null : DateTime.Parse(c[4], null, DateTimeStyles.RoundtripKind),
                ReplacedByTokenHash = string.IsNullOrWhiteSpace(c[5]) ? null : c[5],
                CreatedByIp = string.IsNullOrWhiteSpace(c[6]) ? null : c[6],
                RevokedByIp = string.IsNullOrWhiteSpace(c[7]) ? null : c[7],
                UserAgent = string.IsNullOrWhiteSpace(c[8]) ? null : c[8],
            };
        }

        private static string Csv(string? s)
        {
            s ??= "";
            if (s.Contains('"')) s = s.Replace("\"", "\"\"");
            if (s.Contains(',') || s.Contains('\n') || s.Contains('\r') || s.Contains('"'))
                return $"\"{s}\"";
            return s;
        }

        // minimal CSV parser for one line
        private static List<string> ParseCsvLine(string line)
        {
            var res = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];

                if (inQuotes)
                {
                    if (ch == '"' && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"'); // escaped quote
                        i++;
                    }
                    else if (ch == '"')
                    {
                        inQuotes = false;
                    }
                    else sb.Append(ch);
                }
                else
                {
                    if (ch == ',')
                    {
                        res.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (ch == '"')
                    {
                        inQuotes = true;
                    }
                    else sb.Append(ch);
                }
            }

            res.Add(sb.ToString());
            return res;
        }
    }
}