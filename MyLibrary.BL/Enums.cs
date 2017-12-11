using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    public enum Category
    {
        ArtsAndPhotography,
        BiographiesAndMemoirs,
        Children,
        CookbooksFoodAndWine,
        History,
        LiteratureAndFiction,
        MysteryAndSuspense,
        Romance,
        SciFiAndFantasy,
        TeensAndYoungAdults,
        General
    }

    public enum ArtsAndPhotography
    {
        Architecture,
        BusinessOfArt,
        CollectionsCatalogsAndExhibitions,
        DecorativeArtsAndDesign,
        Drawing,
        Fashion,
        GraphicDesign,
        HistoryAndCriticism,
        IndividualArtists,
        Music,
        OtherMedia,
        Painting,
        PerformingArts,
        PhotographyAndVideo,
        Religious,
        Sculpture,
        StudyAndTeaching,
        VehiclePictorials
    }

    public enum BiographiesAndMemoirs
    {
        ArtsAndLiterature,
        EthnicAndNational,
        Historical,
        LeadersAndNotablePeople,
        Memoirs,
        ProfessionalsAndAcademics,
        ReferenceAndCollections,
        RegionalCanada,
        RegionalUSA,
        SpecificGroups,
        SportsAndOutdoors,
        TravelersAndExplorers,
        TrueCrime
    }

    public enum Children
    {
        Age0To2,
        Age3To5,
        Age6To8,
        Age9To12,
        AwardWinners,
        EducationAndReference,
        BestBooks,
        TeenAndYoungAdult,
        K12Teachers
    }

    public enum CookbooksFoodAndWine
    {
        AsianCooking,
        Baking,
        BeveragesAndWine,
        CanningAndPreserving,
        CelebritiesAndTVShows,
        CookingEducationAndReference,
        CookingMethods,
        CookingByIngredient,
        Desserts,
        EntertainingAndHolidays,
        ItalianCooking,
        KitchenAppliances,
        MainCoursesAndSideDishes,
        OutdoorCooking,
        ProfessionalCooking,
        QuickAndEasy,
        RegionalAndInternational,
        SpecialDiet,
        USACooking,
        VegetarianAndVegan
    }

    public enum History
    {
        Africa,
        Americas,
        ArcticAndAntarctica,
        Asia,
        AustraliaAndOceania,
        Europe,
        MiddleEast,
        Russia,
        UnitedStates,
        World,
        AncientCivilizations,
        Military,
        HistoricalStudyAndEducationalResources
    }

    public enum LiteratureAndFiction
    {
        ActionAndAdventure,
        AfricanAmerican,
        AncientAndMedievalLiterature,
        BritishAndIrish,
        Classics,
        Contemporary,
        DramasAndPlays,
        Erotica,
        EssaysAndCorrespondence,
        ForeignLanguageFiction,
        GenreFiction,
        HistoricalFiction,
        HistoryAndCriticism,
        HumorAndSatire,
        Literary,
        MythologyAndFolkTales,
        Poetry,
        ShortStoriesAndAnthologies,
        UnitedStates,
        WomensFiction,
        WorldLiterature
    }

    public enum MysteryAndSuspense
    {
        Mystery,
        ThrillersAndSuspense,
        Writing
    }

    public enum Romance
    {
        ActionAndAdventure,
        AfricanAmerican,
        Anthologies,
        CleanAndWholesome,
        Contemporary,
        Erotica,
        Fantasy,
        GayRomance,
        Gothic,
        Historical,
        Holidays,
        Inspirational,
        LesbianRomance,
        Military,
        Multicultural,
        NewAdultAndCollege,
        Paranormal,
        Regency,
        RomanticComedy,
        RomanticSuspense,
        ScienceFiction,
        Sports,
        TimeTravel,
        Vampires,
        Western,
        Writing,
        WerewolvesAndShifters
    }

    public enum SciFiAndFantasy
    {
        BestOfTheYear,
        AlternateHistory,
        Epic,
        Historical,
        Military,
        MythsAndlegends,
        Paranormal,
        SwordAndsorcery
    }

    public enum TeensAndYoungAdults
    {
        ArtMusicAndPhotography,
        Biographies,
        EducationAndReference,
        HistoricalFiction,
        HobbiesAndGames,
        LiteratureAndFiction,
        MysteriesAndThrillers,
        PersonalHealth,
        ReligionAndSpirituality,
        Romance,
        ScienceFictionAndFantasy,
        SocialIssues,
        SportsAndOutdoors
    }

    public enum UserType
    {
        Worker,
        Supervisor,
        Manager
    }

    public enum OperationType
    {
        None = 0,
        View = 1,
        Add = 2,
        Edit = 4,
        Delete = 8
    }

    public enum MessageBoxType
    {
        None = 0,
        Error = 16,
        Question = 32,
        Warning = 48,
        Information = 64,
        WarningQuestion = 128
    }

    public enum ValidationType
    {
        RequiredField,
        WrongNumberFormat
    }

    public enum ISBN_Prefix : short
    {
        ISBN_978 = 978,
        ISBN_979 = 979
    }

    public enum ISBN_978_GroupIdentifier
    {
        English = 0,
        French = 2,
        German = 3,
        Japan = 4,
        Russia = 5,
        China = 7,
        Czechoslovakia = 80,
        India = 81,
        Norway = 82,
        Poland = 83,
        Spain = 84,
        Brazil = 85,
        Yugoslavia = 86,
        Denmark = 87,
        Italy = 88,
        RepublicOfKorea = 89,
        Netherlands = 90,
        Sweden = 91,
        InternationalNGOPublishersAndECOrganizations = 92,
        Iran = 600,
        Kazakhstan = 601,
        Indonesia = 602,
        SaudiArabia = 603,
        Vietnam = 604,
        Turkey = 605,
        Romania = 606,
        Mexico = 607,
        Macedonia = 608,
        Lithuania = 609,
        Thailand = 611,
        Peru = 612,
        Mauritius = 613,
        Lebanon = 614,
        Hungary = 615,
        Ukraine = 617,
        Greece = 618,
        Bulgaria = 619,
        Philippines = 621,
        Argentina = 950,
        Finland = 951,
        Croatia = 953,
        SriLanka = 955,
        Chile = 956,
        Taiwan = 957,
        Colombia = 958,
        Cuba = 959,
        Slovenia = 961,
        HongKong = 962,
        Israel = 965,
        Malaysia = 967,
        Pakistan = 969,
        Portugal = 972,
        CARICOM = 976,
        Egypt = 977,
        Nigeria = 978,
        Venezuela = 980,
        Singapore = 981,
        SouthPacific = 982,
        Bangladesh = 984,
        Belarus = 985,
        Cambodia = 9924,
        Cyprus = 9925,
        BosniaAndHerzegovina = 9926,
        Qatar = 9927,
        Albania = 9928,
        Guatemala = 9929,
        Algeria = 9931,
        Laos = 9932,
        Syria = 9933,
        Latvia = 9934,
        Iceland = 9935,
        Afghanistan = 9936,
        Nepal = 9937,
        Armenia = 9939,
        Montenegro = 9940,
        Georgia = 9941,
        Ecuador = 9942,
        Uzbekistan = 9943,
        DominicanRepublic = 9945,
        NorthKorea = 9946,
        UnitedArabEmirates = 9948,
        Estonia = 9949,
        Palestine = 9950,
        Kosova = 9951,
        Azerbaijan = 9952,
        Morocco = 9954,
        Cameroon = 9956,
        Jordan = 9957,
        Libya = 9959,
        Panama = 9962,
        Ghana = 9964,
        Kenya = 9966,
        Kyrgyzstan = 9967,
        CostaRica = 9968,
        Uganda = 9970,
        Tunisia = 9973,
        Uruguay = 9974,
        Moldova = 9975,
        Tanzania = 9976,
        PapuaNewGuinea = 9980,
        Zambia = 9982,
        Gambia = 9983,
        Bahrain = 99901,
        Gabon = 99902,
        NetherlandAntillesAndAruba = 99904,
        Bolivia = 99905,
        Kuwait = 99906,
        Malawi = 99908,
        Malta = 99909,
        SierraLeone = 99910,
        Lesotho = 99911,
        Botswana = 99912,
        Andorra = 99913,
        Suriname = 99914,
        Maldives = 99915,
        Namibia = 99916,
        BruneiDarussalam = 99917,
        FaroeIslands = 99918,
        Benin = 99919,
        ElSalvador = 99923,
        Nicaragua = 99924,
        Paraguay = 99925,
        Honduras = 99926,
        Mongolia = 99929,
        Seychelles = 99931,
        Haiti = 99935,
        Bhutan = 99936,
        Macau = 99937,
        RepublikaSrpska = 99938,
        Sudan = 99942,
        Ethiopia = 99944,
        Tajikistan = 99947,
        Eritrea = 99948,
        Mauritius4 = 99949,
        DemocraticRepublicofCongo = 99951,
        Mali = 99952,
        Luxembourg = 99959,
        Oman = 99969,
        Myanmar = 99971,
        Rwanda = 99977
    }

    public enum ISBN_979_GroupIdentifier
    {
        France = 10,
        RepublicOfKorea = 11,
        Italy = 12
    }
}
