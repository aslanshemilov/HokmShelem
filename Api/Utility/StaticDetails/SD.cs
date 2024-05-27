namespace StaticDetails
{
    public static class SD
    {
        // roles
        public const string Admin = "admin";
        public const string Player = "player";
        public const string Moderator = "moderator";
        public static readonly List<string> Roles = new List<string> { Admin, Player, Moderator };

        public const int MaximumLoginAttempts = 5;

        // External login Providers
        public const string Facebook = "facebook";
        public const string Google = "google";

        // Email send Providers
        public const string SMTP = "SMTP";
        public const string SendGrid = "SendGrid";
        public const string MailJet = "MailJet";

        // Status
        public const string Online = "online";
        public const string Offline = "offline";
        public const string InGame = "in-game";
        public const string Banned = "banned";

        // Game history status
        public const string Completed = "completed";
        public const string Left = "left";
        public const string VotedToEnd = "voted to end";

        public static readonly List<string> Statuses = new List<string> { Online, Offline, InGame, Banned };

        public const string DefaultPassword = "Password123";

        public const string MainLobby = "mainLobby";

        // Color    = Min Value - Max Value
        // Pink     = 0         - 49
        // Purple   = 50        - 99
        // Red      = 100       - 199
        // Green    = 200       - 499
        // Brown    = 500       - 999
        // Blue     = 1000      - 1499
        // Grey     = 1500      - 1999
        // Orange   = 2000      - 2499
        // Black    = 2500      - 2999
        // Diamond  = 3000      - 4999
        // Gold     = 5000      - 10000

        public const string Pink = "pink";
        public const string Purple = "purple";
        public const string Red = "red";
        public const string Green = "green";
        public const string Brown = "brown";
        public const string Blue = "blue";
        public const string Grey = "grey";
        public const string Orange = "orange";
        public const string Diamond = "diamond";
        public const string Gold = "gold";

        public static List<BadgeModel> GetBadges()
        {
            return new List<BadgeModel>
            {
                new BadgeModel(Pink,    0,    49),
                new BadgeModel(Purple,  50,   99),
                new BadgeModel(Red,     100,  199),
                new BadgeModel(Green,   200,  499),
                new BadgeModel(Brown,   500,  999),
                new BadgeModel(Blue,    1000, 1499),
                new BadgeModel(Grey,    1500, 1999),
                new BadgeModel(Orange,  2000, 2499),
                new BadgeModel(Diamond, 2500, 2999),
                new BadgeModel(Gold,    3000, 10000),
            };
        }

        public const string DefaultCountry = "Canada";

        public static List<string> GetCountries()
        {
            return new List<string>()
            {
                "Afghanistan",
                "Argentina",
                "Armenia",
                "Australia",
                "Austria",
                "Azerbaijan",
                "Bahrain",
                "Bangladesh",
                "Belgium",
                "Brazil",
                "Bulgaria",
                "Cameroon",
                "Canada",
                "Chile",
                "China",
                "Colombia",
                "Costa-Rica",
                "Cote-d-Ivoire",
                "Croatia",
                "Cuba",
                "Czech-Republic",
                "Denmark",
                "Dominica",
                "Dominican-Republic",
                "Ecuador",
                "Egypt",
                "Equatorial-Guinea",
                "Finland",
                "France",
                "Georgia",
                "Germany",
                "Ghana",
                "Greece",
                "Hungary",
                "Iceland",
                "India",
                "Indonesia",
                "Iran",
                "Iraq",
                "Ireland",
                "Italy",
                "Jamaica",
                "Japan",
                "Jordan",
                "Kazakhstan",
                "Kuwait",
                "Lebanon",
                "Liberia",
                "Libya",
                "Malaysia",
                "Maldives",
                "Mexico",
                "Mongolia",
                "Nepal",
                "Netherlands",
                "New-Zealand",
                "Nicaragua",
                "Nigeria",
                "Norway",
                "Oman",
                "Pakistan",
                "Paraguay",
                "Peru",
                "Philippines",
                "Poland",
                "Portugal",
                "Qatar",
                "Romania",
                "Russia",
                "Saudi-Arabia",
                "Senegal",
                "Serbia",
                "Singapore",
                "Slovakia",
                "Slovenia",
                "South-Korea",
                "South-Africa",
                "Spain",
                "Sri-Lanka",
                "Sudan",
                "Sweden",
                "Switzerland",
                "Syria",
                "Taiwan",
                "Tajikistan",
                "Tanzania",
                "Thailand",
                "Tunisia",
                "Türkiye",
                "Turkmenistan",
                "Ukraine",
                "United-Arab-Emirates",
                "United-Kingdom",
                "United-States-of-America",
                "Uruguay",
                "Uzbekistan",
                "Venezuela",
                "Vietnam",
                "Yemen",
                "Zimbabwe"
            };
        }

        public static TimeDifferenceDto GetTimeDifferenceFromToday(DateTime endDate)
        {
            DateTime startDate = DateTime.UtcNow;
            TimeSpan difference = endDate - startDate;

            int days = difference.Days;
            int hours = difference.Hours;
            int minutes = difference.Minutes + 1;

            return new TimeDifferenceDto(days, hours, minutes);
        }

        public static string GetBadgeColorByRate(int rate)
        {
            return GetBadges().FirstOrDefault(c => c.MaxValue >= rate && rate >= c.MinValue).Color;
        }

        public static string GetRandomName(int length = 6)
        {
            string result = string.Empty;
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int charactersLength = characters.Length;
            Random random = new Random();

            for (int counter = 0; counter < length; counter++)
            {
                result += characters[random.Next(charactersLength)];
            }

            return result;
        }
    }

    public class BadgeModel
    {
        public BadgeModel()
        {

        }
        public BadgeModel(string color, int minValue, int maxValue)
        {
            Color = color;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public string Color { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}
