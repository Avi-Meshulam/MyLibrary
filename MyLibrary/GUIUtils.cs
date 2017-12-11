using MyLibrary.BL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyLibrary
{
    public enum EMPTY_ENUM { _Empty_ = int.MaxValue}

    static class GUIUtils
    {
        public const string EMPTY_STRING = "{Empty}";

        // In order to satisfy 13-digits-ISBN rules, we must keep 
        // 4 digits for prefix (3 digit) and check digit (1)
        private const int PUBLICATION_SYMBOL_MAX_LENGTH = ISBN.ISBN_LENGTH - 4;

        private static Random _rand = new Random();

        public static ISBN GenerateISBN(
            ISBN_Prefix? prefix = null,
            Enum groupIdentifier = null,
            int publisherCode = -1,
            int catalogueNumber = -1)
        {
            if (prefix == null)
            {
                if (groupIdentifier == null)
                    prefix = _rand.Next(0, 2) == 0 ? ISBN_Prefix.ISBN_978 : ISBN_Prefix.ISBN_979;
                else
                    prefix = groupIdentifier.GetType() == typeof(ISBN_978_GroupIdentifier) ?
                        ISBN_Prefix.ISBN_978 : ISBN_Prefix.ISBN_979;
            }

            if (groupIdentifier == null)
                groupIdentifier = GetRandomGroupIdentifier(prefix);

            int groupIdentifierLength =
                Convert.ChangeType(groupIdentifier, typeof(int)).ToString().Length;

            if (publisherCode == -1)
            {
                int maxPublisherCodeLength =
                PUBLICATION_SYMBOL_MAX_LENGTH
                - groupIdentifierLength
                - 1;    // At least one character for catalogue number

                publisherCode = GetRandomPublisherCode(maxPublisherCodeLength);
            }

            if (catalogueNumber == -1)
            {
                int catalogueNumberLength =
                        PUBLICATION_SYMBOL_MAX_LENGTH
                        - groupIdentifierLength
                        - publisherCode.ToString("D2").Length;  //Publisher code is represented with minimum 2 digits

                int maxCatalogueNumber =
                    (int)Math.Pow(10, catalogueNumberLength) - 1;

                catalogueNumber = _rand.Next(1, maxCatalogueNumber + 1);
            }

            if (prefix == ISBN_Prefix.ISBN_978)
                return new ISBN((ISBN_978_GroupIdentifier)groupIdentifier,
                    publisherCode.ToString(), catalogueNumber.ToString());
            else
                return new ISBN((ISBN_979_GroupIdentifier)groupIdentifier,
                    publisherCode.ToString(), catalogueNumber.ToString());
        }

        private static int GetRandomPublisherCode(int maxLength)
        {
            int publisherCodeLength = _rand.Next(1, maxLength + 1);
            int publisherCode = _rand.Next(
                (int)Math.Pow(10, publisherCodeLength - 1),
                (int)Math.Pow(10, publisherCodeLength)
            );
            return publisherCode;
        }

        public static Enum GetRandomGroupIdentifier(ISBN_Prefix? prefix = null)
        {
            Type groupIdentifierEnum;
            switch (prefix)
            {
                case ISBN_Prefix.ISBN_978:
                    groupIdentifierEnum = typeof(ISBN_978_GroupIdentifier);
                    break;
                case ISBN_Prefix.ISBN_979:
                    groupIdentifierEnum = typeof(ISBN_979_GroupIdentifier);
                    break;
                default:
                    groupIdentifierEnum = (_rand.Next(0, 100) > 10) ?
                        typeof(ISBN_978_GroupIdentifier) : typeof(ISBN_979_GroupIdentifier);
                    break;
            }

            Array groupIdentifiers = Enum.GetValues(groupIdentifierEnum);
            return (Enum)groupIdentifiers.GetValue(_rand.Next(0, groupIdentifiers.Length));
        }

        public static Category GetRandomCategory()
        {
            var categories = CategoriesRepository.Categories;
            return categories.Skip(_rand.Next(0, categories.Count)).First().Key;
        }

        public static Enum GetRandomSubCategory(Category category)
        {
            var categories = CategoriesRepository.Categories;
            return categories[category]?.Skip(_rand.Next(0, categories[category].Count)).First();
        }

        public static DateTime GetRandomDate(int rangeInYears)
        {
            return DateTime.Now.Add(new TimeSpan(-_rand.Next(365, 365 * rangeInYears), 0, 0, 0)).Date;
        }

        public static int CompareNatural(string strA, string strB)
        {
            return CompareNatural(strA, strB, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
        }

        public static int CompareNatural(string strA, string strB, CultureInfo culture, CompareOptions options)
        {
            CompareInfo cmp = culture.CompareInfo;

            int iA = 0;
            int iB = 0;
            int softResult = 0;
            int softResultWeight = 0;

            while (iA < strA.Length && iB < strB.Length)
            {
                bool isDigitA = char.IsDigit(strA[iA]);
                bool isDigitB = char.IsDigit(strB[iB]);

                if (isDigitA != isDigitB)
                {
                    return cmp.Compare(strA, iA, strB, iB, options);
                }
                else if (!isDigitA && !isDigitB)
                {
                    int jA = iA + 1;
                    int jB = iB + 1;
                    while (jA < strA.Length && !char.IsDigit(strA[jA])) jA++;
                    while (jB < strB.Length && !char.IsDigit(strB[jB])) jB++;
                    int cmpResult = cmp.Compare(strA, iA, jA - iA, strB, iB, jB - iB, options);
                    if (cmpResult != 0)
                    {
                        // Certain strings may be considered different due to "soft" differences that are
                        // ignored if more significant differences follow, e.g. a hyphen only affects the
                        // comparison if no other differences follow
                        string sectionA = strA.Substring(iA, jA - iA);
                        string sectionB = strB.Substring(iB, jB - iB);
                        if (cmp.Compare(sectionA + "1", sectionB + "2", options) ==
                            cmp.Compare(sectionA + "2", sectionB + "1", options))
                        {
                            return cmp.Compare(strA, iA, strB, iB, options);
                        }
                        else if (softResultWeight < 1)
                        {
                            softResult = cmpResult;
                            softResultWeight = 1;
                        }
                    }
                    iA = jA;
                    iB = jB;
                }
                else
                {
                    char zeroA = (char)(strA[iA] - (int)char.GetNumericValue(strA[iA]));
                    char zeroB = (char)(strB[iB] - (int)char.GetNumericValue(strB[iB]));
                    int jA = iA;
                    int jB = iB;
                    while (jA < strA.Length && strA[jA] == zeroA) jA++;
                    while (jB < strB.Length && strB[jB] == zeroB) jB++;
                    int resultIfSameLength = 0;
                    do
                    {
                        isDigitA = jA < strA.Length && char.IsDigit(strA[jA]);
                        isDigitB = jB < strB.Length && char.IsDigit(strB[jB]);
                        int numA = isDigitA ? (int)char.GetNumericValue(strA[jA]) : 0;
                        int numB = isDigitB ? (int)char.GetNumericValue(strB[jB]) : 0;
                        if (isDigitA && (char)(strA[jA] - numA) != zeroA) isDigitA = false;
                        if (isDigitB && (char)(strB[jB] - numB) != zeroB) isDigitB = false;
                        if (isDigitA && isDigitB)
                        {
                            if (numA != numB && resultIfSameLength == 0)
                            {
                                resultIfSameLength = numA < numB ? -1 : 1;
                            }
                            jA++;
                            jB++;
                        }
                    }
                    while (isDigitA && isDigitB);
                    if (isDigitA != isDigitB)
                    {
                        // One number has more digits than the other (ignoring leading zeros) - the longer
                        // number must be larger
                        return isDigitA ? 1 : -1;
                    }
                    else if (resultIfSameLength != 0)
                    {
                        // Both numbers are the same length (ignoring leading zeros) and at least one of
                        // the digits differed - the first difference determines the result
                        return resultIfSameLength;
                    }
                    int lA = jA - iA;
                    int lB = jB - iB;
                    if (lA != lB)
                    {
                        // Both numbers are equivalent but one has more leading zeros
                        return lA > lB ? -1 : 1;
                    }
                    else if (zeroA != zeroB && softResultWeight < 2)
                    {
                        softResult = cmp.Compare(strA, iA, 1, strB, iB, 1, options);
                        softResultWeight = 2;
                    }
                    iA = jA;
                    iB = jB;
                }
            }
            if (iA < strA.Length || iB < strB.Length)
            {
                return iA < strA.Length ? 1 : -1;
            }
            else if (softResult != 0)
            {
                return softResult;
            }
            return 0;
        }

        public static string AggregateExceptionMessage(Exception ex)
        {
            var sb = new StringBuilder();
            while (ex != null)
            {
                sb.Append($"{ex.Message}\n\n");
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

        public static MessageBoxResult ShowMessage(string messageBoxText, MessageBoxType messageBoxType)
        {
            MessageBoxButton button = messageBoxType.In(MessageBoxType.Question, MessageBoxType.WarningQuestion) ?
                MessageBoxButton.YesNoCancel : MessageBoxButton.OK;

            if (messageBoxType == MessageBoxType.WarningQuestion)
                messageBoxType = MessageBoxType.Warning;

            string caption = Enum.GetName(typeof(MessageBoxType), messageBoxType);

            return MessageBox.Show(messageBoxText, caption, button, (MessageBoxImage)messageBoxType);
        }

        public static string GetRandomFirstName()
        {
            return _firstNames[_rand.Next(0, _firstNames.Count)];
        }

        public static string GetRandomLastName()
        {
            return _lastNames[_rand.Next(0, _lastNames.Count)];
        }

        public static string GetRandomPublisherName()
        {
            return _publishersNames[_rand.Next(0, _publishersNames.Count)];
        }

        #region Data Lists
        private static List<string> _firstNames = new List<string>
        {
            "James", "John", "Robert", "Michael", "William", "David", "Richard", "Charles", "Joseph", "Thomas", "Christopher", "Daniel", "Paul", "Mark", "Donald", "George", "Kenneth", "Steven", "Edward", "Brian", "Ronald", "Anthony", "Kevin", "Jason", "Matthew", "Gary", "Timothy", "Jose", "Larry", "Jeffrey", "Frank", "Scott", "Eric", "Stephen", "Andrew", "Raymond", "Gregory", "Joshua", "Jerry", "Dennis", "Walter", "Patrick", "Peter", "Harold", "Douglas", "Henry", "Carl", "Arthur", "Ryan", "Roger", "Mary", "Patricia", "Linda", "Barbara", "Elizabeth", "Jennifer", "Maria", "Susan", "Margaret", "Dorothy", "Lisa", "Nancy", "Karen", "Betty", "Helen", "Sandra", "Donna", "Carol", "Ruth", "Sharon", "Michelle", "Laura", "Sarah", "Kimberly", "Deborah", "Jessica", "Shirley", "Cynthia", "Angela", "Melissa", "Brenda", "Amy", "Anna", "Rebecca", "Virginia", "Kathleen", "Pamela", "Martha", "Debra", "Amanda", "Stephanie", "Carolyn", "Christine", "Marie", "Janet", "Catherine", "Frances", "Ann", "Joyce", "Diane"
        };

        private static List<string> _lastNames = new List<string>
        {
            "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter", "Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed", "Cook", "Morgan", "Bell", "Murphy", "Bailey", "Rivera", "Cooper", "Richardson", "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly", "Sanders", "Price", "Bennett", "Wood", "Barnes", "Ross", "Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long", "Patterson", "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales", "Bryant", "Alexander", "Russell", "Griffin", "Diaz", "Hayes"
        };

        private static List<string> _publishersNames = new List<string>
        {
            "Pearson PLC", "The Woodbridge Company Ltd.", "Reed Elsevier PLC & Reed Elsevier NV", "Wolters Kluwer", "Bertelsmann AG", "China South Publishing & Media Group Co., Ltd", "Phoenix Publishing and Media Company", "Lagardère", "Apollo Global Management LLC", "Grupo Planeta", "Wiley", "Scholastic", "News Corp.", "Apax and Omers Capital Partners", "Holtzbrinck & EQT and GIC Investors", "Houghton Mifflin Harcourt Company", "China Publishing Group Corporation", "Zhejiang Publishing United Group", "Verlagsgruppe Georg von Holtzbrinck", "China Education Publishing & Media Holdings Co. Ltd.", "Oxford University", "Informa plc", "Hitotsubashi Group", "Kadokawa Holdings Inc.", "Kodansha Ltd.", "Hitotsubashi Group", "The Bonnier Group", "Egmont International Holding A/S", "CBS", "PRISA SA", "Woongjin Holding", "Klett Gruppe", "Messagerie Italiane", "Gruppo De Agostini", "Madrigall", "Frojal", "Cambridge University Press", "Media Participations", "The Mondadori Group", "Medien Union", "Sanoma WSOY", "Cornelsen", "Privately owned", "Kyowon Co. Ltd.", "WEKA Firmengruppe", "La Martinière Groupe", "Gakken Co. Ltd.", "Privately owned", "Privately owned", "Bungeishunju Ltd.", "Groupe Albin Michel", "Shinchosa Publishing Co, Ltd."
        };

        public static List<Tuple<string, string>> BooksAndAuthors = new List<Tuple<string, string>>
        {
            Tuple.Create("Don Quixote","Miguel De Cervantes"),
            Tuple.Create("Pilgrim's Progress","John Bunyan"),
            Tuple.Create("Robinson Crusoe","Daniel Defoe"),
            Tuple.Create("Gulliver's Travels","Jonathan Swift"),
            Tuple.Create("Tom Jones","Henry Fielding"),
            Tuple.Create("Clarissa","Samuel Richardson"),
            Tuple.Create("Tristram Shandy","Laurence Sterne"),
            Tuple.Create("Dangerous Liaisons","Pierre Choderlos De Laclos"),
            Tuple.Create("Emma","Jane Austen"),
            Tuple.Create("Frankenstein","Mary Shelley"),
            Tuple.Create("Nightmare Abbey","Thomas Love Peacock"),
            Tuple.Create("The Black Sheep","Honoré De Balzac"),
            Tuple.Create("The Charterhouse of Parma","Stendhal"),
            Tuple.Create("The Count of Monte Cristo","Alexandre Dumas"),
            Tuple.Create("Sybil","Benjamin Disraeli"),
            Tuple.Create("David Copperfield","Charles Dickens"),
            Tuple.Create("Wuthering Heights","Emily Brontë"),
            Tuple.Create("Jane Eyre","Charlotte Brontë"),
            Tuple.Create("Vanity Fair","William Makepeace Thackeray"),
            Tuple.Create("The Scarlet Letter","Nathaniel Hawthorne"),
            Tuple.Create("Moby-Dick","Herman Melville"),
            Tuple.Create("Madame Bovary","Gustave Flaubert"),
            Tuple.Create("The Woman in White","Wilkie Collins"),
            Tuple.Create("Alice's Adventures In Wonderland","Lewis Carroll"),
            Tuple.Create("Little Women","Louisa M. Alcott"),
            Tuple.Create("The Way We Live Now","Anthony Trollope"),
            Tuple.Create("Anna Karenina","Leo Tolstoy"),
            Tuple.Create("Daniel Deronda","George Eliot"),
            Tuple.Create("The Brothers Karamazov","Fyodor Dostoevsky"),
            Tuple.Create("The Portrait of a","Lady Henry James"),
            Tuple.Create("Huckleberry Finn","Mark Twain"),
            Tuple.Create("The Strange Case of Dr Jekyll and Mr Hyde","Robert Louis Stevenson"),
            Tuple.Create("Three Men in a Boat","Jerome K. Jerome"),
            Tuple.Create("The Picture of Dorian Gray","Oscar Wilde"),
            Tuple.Create("The Diary of a Nobody","George Grossmith"),
            Tuple.Create("Jude the Obscure","Thomas Hardy"),
            Tuple.Create("The Riddle of the Sands","Erskine Childers"),
            Tuple.Create("The Call of the Wild","Jack London"),
            Tuple.Create("Nostromo","Joseph Conrad"),
            Tuple.Create("The Wind in the Willows","Kenneth Grahame"),
            Tuple.Create("In Search of Lost Time","Marcel Proust"),
            Tuple.Create("The Rainbow","D. H. Lawrence"),
            Tuple.Create("The Good Soldier","Ford Madox Ford"),
            Tuple.Create("The Thirty-Nine","Steps John Buchan"),
            Tuple.Create("Ulysses","James Joyce"),
            Tuple.Create("Mrs Dalloway","Virginia Woolf"),
            Tuple.Create("A Passage to India","EM Forster"),
            Tuple.Create("The Great Gatsby","F. Scott Fitzgerald"),
            Tuple.Create("The Trial","Franz Kafka"),
            Tuple.Create("Men Without Women","Ernest Hemingway"),
            Tuple.Create("Journey to the End of the Night","Louis-Ferdinand Celine"),
            Tuple.Create("As I Lay Dying","William Faulkner"),
            Tuple.Create("Brave New World","Aldous Huxley"),
            Tuple.Create("Scoop","Evelyn Waugh"),
            Tuple.Create("USA John","Dos Passos"),
            Tuple.Create("The Big Sleep","Raymond Chandler"),
            Tuple.Create("The Pursuit Of Love","Nancy Mitford"),
            Tuple.Create("The Plague","Albert Camus"),
            Tuple.Create("Nineteen Eighty-Four","George Orwell"),
            Tuple.Create("Malone Dies","Samuel Beckett"),
            Tuple.Create("Catcher in the Rye","J.D. Salinger"),
            Tuple.Create("Wise Blood","Flannery O'Connor"),
            Tuple.Create("Charlotte's Web","EB White"),
            Tuple.Create("The Lord Of The Rings","J. R. R. Tolkien"),
            Tuple.Create("Lucky Jim","Kingsley Amis"),
            Tuple.Create("Lord of the Flies","William Golding"),
            Tuple.Create("The Quiet American","Graham Greene"),
            Tuple.Create("On the Road","Jack Kerouac"),
            Tuple.Create("Lolita","Vladimir Nabokov"),
            Tuple.Create("The Tin Drum","Günter Grass"),
            Tuple.Create("Things Fall Apart","Chinua Achebe"),
            Tuple.Create("The Prime of Miss Jean Brodie","Muriel Spark"),
            Tuple.Create("To Kill A Mockingbird","Harper Lee"),
            Tuple.Create("Catch-22","Joseph Heller"),
            Tuple.Create("Herzog","Saul Bellow"),
            Tuple.Create("One Hundred Years of Solitude","Gabriel García Márquez"),
            Tuple.Create("Mrs Palfrey at the Claremont","Elizabeth Taylor"),
            Tuple.Create("Tinker Tailor Soldier Spy","John Le Carré"),
            Tuple.Create("Song of Solomon","Toni Morrison"),
            Tuple.Create("The Bottle Factory Outing","Beryl Bainbridge"),
            Tuple.Create("The Executioner's Song","Norman Mailer"),
            Tuple.Create("If on a Winter's Night a Traveller","Italo Calvino"),
            Tuple.Create("A Bend in the River","VS Naipaul"),
            Tuple.Create("Waiting for the Barbarians","JM Coetzee"),
            Tuple.Create("Housekeeping","Marilynne Robinson"),
            Tuple.Create("Lanark","Alasdair Gray"),
            Tuple.Create("The New York Trilogy","Paul Auster"),
            Tuple.Create("The BFG","Roald Dahl"),
            Tuple.Create("The Periodic Table","Primo Levi"),
            Tuple.Create("Money","Martin Amis"),
            Tuple.Create("An Artist of the Floating World","Kazuo Ishiguro"),
            Tuple.Create("Oscar And Lucinda","Peter Carey"),
            Tuple.Create("The Book of Laughter and Forgetting","Milan Kundera"),
            Tuple.Create("Haroun and the Sea of Stories","Salman Rushdie"),
            Tuple.Create("LA Confidential","James Ellroy"),
            Tuple.Create("Wise Children","Angela Carter"),
            Tuple.Create("Atonement","Ian McEwan"),
            Tuple.Create("Northern Lights","Philip Pullman"),
            Tuple.Create("American Pastoral","Philip Roth"),
            Tuple.Create("Austerlitz","W. G. Sebald")
        };

        public static List<Tuple<string, string>> JournalsAndEditors = new List<Tuple<string, string>>
        {
            Tuple.Create("Journal of Nanoscience and Nanotechnology", "Dr. Hari Singh Nalwa"),
            Tuple.Create("Journal of Computational and Theoretical Nanoscience", "Prof. Dr. Wolfram Schommers"),
            Tuple.Create("Journal of Biomedical Nanotechnology", "Dr. Omathanu Perumal"),
            Tuple.Create("Journal of Nanoelectronics and Optoelectronics", "Dr. Ahmad Umar"),
            Tuple.Create("Journal of Low Power Electronics", "Dr. Patrick GIRARD"),
            Tuple.Create("Sensor Letters", "Dr. Ahmad Umar"),
            Tuple.Create("Journal of Nano Education", "Dr. Kurt Winkelmann"),
            Tuple.Create("Journal of Nanoneuroscience", "Prof. Dr. Zhihua Cui"),
            Tuple.Create("American Journal of Neuroprotection and Neuroregeneration", "Prof. Dr. Hari Shanker Sharma"),
            Tuple.Create("Journal of Colloid Science and Biotechnology", "Dr. Abdelhamid Elaissari, France"),
            Tuple.Create("Journal of Bionanoscience", "Dr. Murugan Ramalingam, Prof. Dr. Nongyue He"),
            Tuple.Create("Journal of Biobased Materials and Bioenergy", "Professor Shijie Liu, Prof. Dr. Nongyue He, P.R."),
            Tuple.Create("Advanced Science,  Engineering and Medicine", "Dr. Hari Singh Nalwa"),
            Tuple.Create("Journal of Advanced Microscopy Research", "Professor Dr. Vladimir A. Basiuk "),
            Tuple.Create("Journal of Holography and Speckle", "Dr. A. R. Ganesan and Dr. Murukeshan Vadakke Matham"),
            Tuple.Create("Nanoscience and Nanotechnology Letters", "Prof. Dr. Nongyue He"),
            Tuple.Create("Advanced Science Letters", "Dr. Hari Singh Nalwa"),
            Tuple.Create("Science of Advanced Materials", "Dr. Ahmad Umar"),
            Tuple.Create("Journal of Nano Energy and Power Research", "Dr. R. Jeyakumar, PhD"),
            Tuple.Create("Journal of Advanced Mathematics and Applications", "Professor Yingxu Wang "),
            Tuple.Create("Journal of Medical Imaging and Health Informatics", "Dr. Eddie Yin-Kwee"),
            Tuple.Create("Journal of Biomaterials and Tissue Engineering", "Dr. Murugan Ramalingam"),
            Tuple.Create("Reviews in Nanoscience and Nanotechnology", "Dr. Wei Chen"),
            Tuple.Create("Reviews in Advanced Sciences and Engineering", "Dr. Ahmad Umar"),
            Tuple.Create("Journal of Nanoengineering and Nanomanufacturing", "Dr. Ahmad Umar"),
            Tuple.Create("Journal of Mechatronics", "Dr. Ruidan Su"),
            Tuple.Create("Advanced Electrochemistry", "Dr. Marshal Dhayal"),
            Tuple.Create("Messenger", "Professor Hon Cheung Lee"),
            Tuple.Create("Journal of Computational Intelligence and Electronic Systems", "Dr. Ruidan Su"),
            Tuple.Create("Journal of Spintronics and Magnetic Nanomaterials", "Professor Dr. Julián González"),
            Tuple.Create("Advanced Science Focus", "Professor K. D. Verma"),
            Tuple.Create("Materials Focus", "Dr. Nikolaos Bouropoulos, PhD"),
            Tuple.Create("Journal of Energy and Environmental Technology", "Prof. Dr. Yuncai Zhou "),
            Tuple.Create("Journal of Bioinformatics and Intelligent Control", "Prof. Dr. Zhihua Cui"),
            Tuple.Create("Journal of Nanopharmaceutics and Drug Delivery", "Dr. Omathanu Perumal"),
            Tuple.Create("Journal of Neuroscience and Neuroengineering", "Dr. Lun-De Liao, Dr. Chin-Teng Lin"),
            Tuple.Create("Journal of Nanofluids", "Prof. John Philip, PhD"),
            Tuple.Create("Journal of Surfaces and Interfaces of Materials", "Prof. Dr. Constantin Politis, Prof. Dr. Efstathios I. Meletis and Prof. Dr. Wolfram Schommers"),
            Tuple.Create("Advanced Natural Products", "Prof. Dr. Aranya Manosroi"),
            Tuple.Create("Journal of Nutritional Ecology and Food Research", "Prof. Zhiyong Qian"),
            Tuple.Create("Nano Communications", "Dr. Shashadhar Samal, PhD, DSc"),
            Tuple.Create("Advanced Carbon", "Dr. Avinash Balakrishnan"),
            Tuple.Create("Journal of Sensing and Quantification", "Prof. Craig A. Grimes"),
            Tuple.Create("Journal of Agriculture and Animal Science", "Dr.Gautam Kaul"),
            Tuple.Create("Advanced Porous Materials", "Prof. Ajayan Vinu"),
            Tuple.Create("Journal of Mutifunctional Polymers", "Prof. Dr. Sabu Thomas"),
            Tuple.Create("Journal of Chitin and Chitosan Science", "Prof. R. Jayakumar "),
            Tuple.Create("Journal of Biopharmaceutics and Biotechnology", "Prof. R. Jayakumar"),
            Tuple.Create("Reviews in Theoretical Science", "Prof. Dr. Wolfram Schommers"),
            Tuple.Create("Journal of Metals and Metallurgy", "Prof. Kaiming Wu"),
            Tuple.Create("Graphene", "Prof. Sundara Ramaprabhu"),
            Tuple.Create("Journal of Coupled Systems and Multiscale Dynamics", "Prof. Dr. Roderick Melnik"),
            Tuple.Create("Neurosurgical Science", "Prof. Lukui Chen, and Prof. Robert F. Spetzler"),
            Tuple.Create("Journal of Hydrogels", "Professor Marcel POPA, PhD "),
            Tuple.Create("American Journal of Robotic Surgery", "Dinesh Vyas, MD, MS, FACS"),
            Tuple.Create("Journal of Informatics Cybernetics and Telemetics", "Prof. Ford Lumban Gaol"),
            Tuple.Create("Journal of Systems Analysis and Software Engineering", "Prof. Anurag Srivastava"),
            Tuple.Create("Journal of Computational Biology, Biotechnology and Machine Learning", "Dr. Hamid Alinejad-Rokny and Dr. Mohammad Javad Rasaee")
        };
        #endregion  // Data Lists
    }
}