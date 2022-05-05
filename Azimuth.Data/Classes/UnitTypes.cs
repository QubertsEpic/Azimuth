using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.Classes
{
    public static class UnitTypes
    {
        //Damn american spellings.
        public const string Meter = "meter";
        public const string MeterScalar256 = "meter scaler 256";
        public const string Millimeter = "millimeter";
        public const string Centimeter = "centimeter";
        public const string Kilometer = "kilometer";
        public const string NauticalMile = "nautical mile";
        public const string Decinmile = "decinmile";
        public const string Inch = "inch";
        public const string Foot = "foot";
        public const string Yard = "yard";
        public const string Decimile = "decimile";
        public const string Mile = "mile";

        public const string SquareInch = "square inch";
        public const string SquareFoot = "square foot";
        public const string SquareYard = "square yard";
        public const string SquareMile = "square mile";
        public const string SquareMillimeter = "square millimeter";
        public const string SquareCentimeter = "square centimeter";
        public const string SquareMeter = "square meter";
        public const string SquareKilometer = "square kilometer";

        public const string CubicInch = "cubic inch";
        public const string CubicFoot = "cubic foot";
        public const string CubicYard = "cubic yard";
        public const string CubicMile = "cubic mile";
        public const string CubicMillimeter = "cubic millimeter";
        public const string CubicCentimeter = "cubic centimeter";
        public const string CubicMeter = "cubic meter";
        public const string CubicKilometer = "cubic kilometer";
        public const string Liter = "liter";
        public const string Gallon = "gallon";
        public const string Quart = "quart";

        public const string Kelvin = "kelvin";
        public const string Rankine = "rankine";
        public const string Farenheit = "farenheit";
        public const string Celcius = "celcius";
        public const string CelciusFs7OilTemp = "celcius fs7 oil temp";
        public const string CelciusFs7Egt = "celcius fs7 egt";
        public const string CelciusScaler1256 = "celcius scaler 1/256";
        public const string CelciusScaler256 = "celcius scaler 256";
        public const string CelciusScaler16k = "celcius scaler 16k";

        public const string Radian = "radian";
        public const string Round = "round";
        public const string Degree = "degree";
        public const string Gradian = "grad";
        public const string Angle16 = "angl16";
        public const string Angle32 = "angle32";

        public const string DegreeLatitude = "degree latitude";
        public const string DegreeLongitude = "degree longitude";
        public const string MeterLatitude = "meter latitude";

        public const string RadiansPerSecond = "radians per second";
        public const string RevolutionsPerSecond = "revolutions per second";
        public const string Rpm1Over16k = "rpm 1 over 16k";
        public const string MinutePerRound = "minute per round";
        public const string NiceMinutePerRound = "nice minute per round";
        public const string DegreesPerSecond = "degree per second";
        public const string DegreesPerSecondAng16 = "degree per second ang16";

        public const string RadiansPerSecondSquared = "radians per second squared";
        public const string DegreesPerSecondSquared = "degrees per second squared";

        public const string MetersPerSecond = "m/s";
        public const string MetersPerSecondScaler = "meters per second scaler";
        public const string MetersPerMinute = "meters per minute";
        public const string KilometersPerHour = "kph";
        public const string FeetPerSecond = "feet/second";
        public const string FeetPerMinute = "feet/minute";
        public const string MilesPerHour = "mile per hour";
        public const string Knots = "knots";
        public const string KnotsScaler128 = "knots scaler 128";
        public const string Machs = "machs";
        public const string Machs3d2Over64k = "mach 3d2 over 64k";

        public const string MetersPerSecondSquared = "meters per second squared";
        public const string FeetPerSecondSquared = "feet per second squared";
        public const string GForce = "Gforce";
        public const string GForce624Scaled = "G Force 624 scaled";

        public const string Second = "second";
        public const string Minute = "minute";
        public const string Hour = "hour";
        public const string Day = "day";
        public const string HourOver10 = "hour over 10";
        public const string Year = "year";

        public const string Watt = "watt";
        public const string FootPoundsPerSecond = "ft lb per second";

        public const string MeterCubedPerSecond = "meter cubed per second";
        public const string LiterPerHour = "liter per hour";
        public const string GallonPerHour = "gallon per hour";

        public const string Kilogram = "kilogram";
        public const string Pound = "lbs";
        public const string PoundScaler256 = "pound scaler 256";
        public const string Slug = "slug";

        public const string KilogramPerSecond = "kilogram per second";
        public const string PoundPerHour = "pound per hour";

        public const string Amepre = "ampere";
        public const string Fs7ChargingAmps = "fs7 charging amps";

        public const string Volts = "volt";

        public const string Hertz = "Hz";
        public const string KiloHertz = "KHz";
        public const string MegaHertz = "MHz";
        public const string FrequencyBCD16 = "Frequency BCD16";
        public const string FrequencyBCD32 = "Frequency BCD32";
        public const string FrequencyADFBCD32 = "Frequency ADF BCD32";

        public const string KilogramsPerCubicMeter = "kilogram per cubic meter";
        public const string SlugPerCubicFoot = "slug per cubic foot";
        public const string PoundPerGallon = "pound per gallon";

        public const string Pascal = "pascal";
        public const string KiloPascal = "kilopascal";
        public const string MillimeterOfMercury = "millimeter of mercury";
        public const string CentimeterOfMercury = "centimeter of mercury";
        public const string InchOfMercury = "inch of mercury";
        public const string MillimetersOfWater = "millimeter of water";
        public const string NewtonPerSquareMeter = "Newtom per square meter";
        public const string KilogramForcePerSquareCentimeter = "kilogram force per square centimeter";
        public const string KilogramMeterSquared = "kilogram meter squared";
        public const string Atmospheres = "atmosphere";
        public const string Bar = "bar";
        public const string MilliBar = "millibar";
        public const string MilliBarScaler = "millibar scaler 16";
        public const string PoundForcePerSquareInch = "psi";
        public const string PSIScaler = "psi scaler 16k";
        public const string PSIScaler4over16 = "psi 4 over 16k";
        public const string PSIFS7OilPressure = "psi fs7 oil presssure";
        public const string PoundForcePerSquareFoot = "psf";
        public const string PSFScaler = "psf scaler 16k";
        public const string SlugsFeetSquared = "slug feet squared";
        public const string BoostcmHg = "boost cmHg";
        public const string BoostinHg = "boost inHg";
        public const string BoostPSI = "boost psi";

        public const string NewtonMeter = "nm";
        public const string FootPound = "food-pound";
        public const string PoundFoot = "lbf-feet";
        public const string KilogramForceMeter = "kgf";
        public const string PoundalFeet = "poundal feet";

        public const string FractionalLatitideLonitudeDigits = "FractionalLatLonDigits";
        public const string Part = "part";
        public const string Half = "half";
        public const string Third = "third";
        public const string Percent = "percent";
        public const string PercentOver100 = "percent over 100";
        public const string PercentScaler16k = "percent scaler 16k";
        public const string PercentScaler32k = "percent scaler 32k";
        public const string PercentScaler2Exp32 = "percent scaler 2pow23";
        public const string Bel = "bel";
        public const string Decibel = "decibel";
        public const string MoreThanAHalf = "more_than_a_half";
        public const string Times = "times";
        public const string Ratio = "ratio";
        public const string Number = "number";
        public const string Scaler = "scaler";
        public const string Position = "position";
        public const string Enum = "Enum";
        public const string Bool = "bool";
        public const string Bco16 = "Bco16";
        public const string Mask = "mask";
        public const string Flags = "flags";
        public const string String = "string";
        public const string PerRadian = "per radian";
        public const string PerDegree = "per degree";
        public const string Keyframe = "keyframe";

        public const string XYZ = "XYZ";
        public const string PitchBankHeading = "PBH";
        public const string LatLonAltPitchBankHeading = "latlonaltpbh";
        public const string LatLonAlt = "latlonalt";
        public const string PIDStruct = "PID_STRUCT";
        public const string POIList = "POIList";
        public const string GlassCockpitSettings = "GlassCockpitSettings";
        public const string FuelLevels = "FuelLevels";
    }
}
