using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Dynamic;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }

    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");

        // copy of token generator from defs for testing
        static readonly char[] AvailableCharacters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-'
        };

        public string GenerateTokenString(int length)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            //randomize
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            //use randomized data to create token
            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            //return token
            return new string(identifier);
        }

        [TestMethod]
        public void Test001_CreateUser()
        {
            dynamic user = new ExpandoObject();

            user.Nickname = "@John";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            user.Nickname = "#John";
            r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(NotImplemented, r.Status);
            
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            user.Nickname = "";
            r = client.DoPostAsync("users", user).Result;
            // bad request
            Assert.AreEqual(Forbidden, r.Status);
        }

        [TestMethod]
        public void Test002_JoinGame()
        {
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();

            // token not there
            gameInput.UserToken = GenerateTokenString(40);
            gameInput.TimeLimit = 60;
            Response r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // time limit too low
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 0;
            r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // time limit too high
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 100000;
            r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // pending game - created game
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            r = client.DoPostAsync("games", gameInput).Result;
            user.Nickname = "Sally";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            r = client.DoPostAsync("games", gameInput).Result;
            Assert.AreEqual(Created, r.Status);

            // not sure how to test a generated token collision, 
            // but it needs to happen for code coverage
        }

        [TestMethod]
        public void Test003_CancelJoin()
        {
            dynamic user = new ExpandoObject();
            dynamic userID = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();

            // null user
            Response r = client.DoPutAsync(null, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // no pending game
            userID.UserToken = GenerateTokenString(40);
            r = client.DoPutAsync(userID, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // not the same guy
            user.Nickname = "John";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            var userID2 = r.Data;
            r = client.DoPostAsync("games", gameInput).Result;
            user.Nickname = "notJohn";
            r = client.DoPutAsync(client.DoPostAsync("users", user).Result.Data, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // cancel success
            r = client.DoPutAsync(userID2, "games").Result;
            Assert.AreEqual(OK, r.Status);
        }

        [TestMethod]
        public void Test004_PlayWord()
        {
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();
            dynamic wordInput = new ExpandoObject();

            // null playword object
            Response r = client.DoPutAsync(null, "games/{GameID}").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // null gameid
            wordInput.UserToken = null;
            wordInput.Word = null;
            r = client.DoPutAsync(wordInput, "games/{GameID}").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }
        [TestMethod]
        public void Test005_PlayWord()
        {
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();
            dynamic wordInput = new ExpandoObject();

            user.Nickname = "John";
            Response r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            var userID = r.Data;
            r = client.DoPostAsync("games", gameInput).Result;
            user.Nickname = "Sally";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 60;
            r = client.DoPostAsync("games", gameInput).Result;
            string gameID = r.Data.GameID;

            // UserToken null
            wordInput.UserToken = null;
            wordInput.Word = "hello";
            r = client.DoPutAsync(wordInput, "games/{GameID}").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // Word null
            wordInput.UserToken = userID.UserToken;
            wordInput.Word = null;
            r = client.DoPutAsync(wordInput, "games/{GameID}").Result;
            Assert.AreEqual(Forbidden, r.Status);

            // no game
            wordInput.UserToken = userID.UserToken;
            wordInput.Word = "hello";
            r = client.DoPutAsync(wordInput, "games/" + GenerateTokenString(40)).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // with the id but not a player in game
            user.Nickname = "Alex";
            r = client.DoPostAsync("users", user).Result;
            var userID2 = r.Data;
            wordInput.UserToken = userID2.UserToken;
            wordInput.Word = "hello";
            r = client.DoPutAsync(wordInput, "games/" + gameID).Result;
            Assert.AreEqual(Forbidden, r.Status);

            // with the id
            wordInput.UserToken = userID.UserToken;
            wordInput.Word = "hello";
            r = client.DoPutAsync(wordInput, "games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);
        }
        [TestMethod]
        public void Test007_GameStatus()
        {
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();
            dynamic wordInput = new ExpandoObject();

            user.Nickname = "John";
            Response r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 6;
            var user1ID = r.Data;
            r = client.DoPostAsync("games", gameInput).Result;
            var gameID = r.Data.GameID;

            r = client.DoGetAsync("games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);

            r = client.DoGetAsync("games/" + GenerateTokenString(40)).Result;
            Assert.AreEqual(Forbidden, r.Status);

            user.Nickname = "Sally";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 6;
            var user2ID = r.Data;
            r = client.DoPostAsync("games", gameInput).Result;
            gameID = r.Data.GameID;
            
            wordInput.UserToken = user1ID.UserToken;
            wordInput.Word = "hello";
            r = client.DoPutAsync(wordInput, "games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);
            /*
            string[] myarray = { "" };
            String url = String.Format("games/" + gameID + "?Brief={0}", myarray);
            r = client.DoGetAsync(url).Result;
            */
            r = client.DoGetAsync("games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);

            System.Threading.Thread.Sleep(12000);

            r = client.DoGetAsync("games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);

            r = client.DoGetAsync("games/" + GenerateTokenString(40)).Result;
            Assert.AreEqual(Forbidden, r.Status);
            
            r = client.DoGetAsync("games/" + gameID + "?Brief={0}", "Yes").Result;
            Assert.AreEqual(OK, r.Status);

            r = client.DoGetAsync("games/" + gameID + "?Brief={0}", "Yes").Result;
            Assert.AreEqual(OK, r.Status);
        }
        [TestMethod]
        public void Test009_LotsOfWords()
        {
            dynamic user = new ExpandoObject();
            dynamic gameInput = new ExpandoObject();
            dynamic wordInput = new ExpandoObject();

            user.Nickname = "John";
            Response r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 30;
            var user1ID = r.Data;
            r = client.DoPostAsync("games", gameInput).Result;
            var gameID = r.Data.GameID;

            user.Nickname = "Sally";
            r = client.DoPostAsync("users", user).Result;
            gameInput.UserToken = r.Data.UserToken;
            gameInput.TimeLimit = 30;
            var user2ID = r.Data;
            r = client.DoPostAsync("games", gameInput).Result;
            gameID = r.Data.GameID;

            r = client.DoGetAsync("games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);
            string board = r.Data.Board;

            string lyrics = @"number theoretic Pitiable Sherezer on the sly Lombardeer tethered substance cluster of galaxies s.a.e. cuq oodles unamplified Fatherliness Haughtiness tub gurnard peritonitis Crimplene flyboy black letter Calapuya Clitocybe irina graduate student D23 slip stitch alumroot 42TX Mat rush Pectoriloquial Adipocere Bang's disease vine maple simple phobia hot spring 0OK4 Enchafed photoreactivating cavitary Centriscus scolopax Equiparable Teste Case Abuttal -taxy Feed wheel Platymiscium pinnatum inguen bird cherry tree reenforced concrete battue Wicklow TELEM stomodaeal scientific notation axiomatization sink without trace Cypher Crimson NE35 self-suggestion Adduced in bad odor exhilaratingly chaff househusband convolvulus previsional lysis Prize fighter zucchetto red poll old-line JOSEPH BARSABBAS open-face 8PA7 kegful wych-alder silver screen Joint splice scatter-brains parodistic hyperkinetic sulfur mustard mecar family Characeae John III Ducas Vatatzes Wretchless lip-shaped Sacchulmic Cobnut thermoregulate To keep counsel Foliole Gritstone Pheidias Pleuras wimp 99Z tacet tea-party embranglement agar ocean station ship yellow pea Eriocaulon Screw bolt Ferventness safety lane S paradoxus dischargee cavalier hat Serving mallet Tallest feather in the cap demented Dance Affluent Mosca paper feed side-by-side red baneberry Wordish Chelone glabra Anthemwise 7IS1 lithosphere Silurus Medgar Wiley Evers Conchometer genus Eschrichtius Jaculated orphanhood Furhelow Way-going crop Friedman diary -atic vermillion psychological testing mechanics Herakline SD65 honey stomach backcloth autacoidal factitive Corymbulous Plenipotentiary PARCHED CORN (GRAIN) To open up dryad servant-maid nonsegmental Impenitently Merrymake SSW 7MO1 cook wrasse pre-position mix in in loco parentis godwit Al-Hasan ibn al-Haytham Rhombogene Nectocalyces Jerusalem Warriors Menshevik carbon 14 spearwort Crosswort Cytoblast Gospel-gossip NC75 genus Phytolacca organized crime sarcosporidian celery-leaved buttercup studhorse truditur dies die Jacaranda ovalifolia disability of walking motor control buttermilk biscuit Scaphiopus multiplicatus Hall effect Prelatical battle of Plassey W36 Abbreuvoir ethanoic acid Pinkster flower humous IYS witching megacity atactic Adjudicature isotopic entrammel chlorination Quenching Hoffmannsthal mid-March 78IN C6H3CO23 phraseology aortography Expertly stylize Salmons Biafran plant order public orator mark up leave out anti- Birt Umbilic Gabbier ribosome rehandle Mountain Standard Time Choroid ficikeb Electrization Diploetic hovering ceiling Upsterte grigri palm Wavure osteoarthritic positron-emission tomography brother or sister anaesthetist Stanley Steamer Uvularia grandiflora demineralize Butyrometer anamnesis facies 5P3 Bonedog Plained toe dancer home loan 39NC Antonius SDP nonpathogenic Alfred de Musset readapt emissary Exemplifying Henri Labrouste insurrectionist Homoeomorphous mekac Ductor PA97 construable digamma Limaceous Fehm sewability Fanti biological diversity 97M dashed hope roentgenologic metralgia 77T C20 4IL3 unashamed folnos Ipomoea alba editorialization implements of war thriftshop MO64 Busiless candytuft Deseret Dermatobia hominis unmerciful sit on Effectuating Bagdad ring rot fungus gangsta lapsus linguae paleolith overextension make of Lecturer Laestadia Bidwellii Rain goose Dick Fosbury stade amp(1) DKX ID19 Ventro-inguinal Dvergr nugget Bluegrass Region Diaphemetric Turtle-footed Pedal organ base line puka Coly genus Hydrastis ethoxyethane packability 57PA field mint get a noseful Maksim Gorky Chained deputise St. Vitus dance Shoppish Hogben iguanid lizard Coreopsis gigantea bear's ear Inordination amenably TIF Nymphaea stellata onionskin Chlorin Efforce Billie the Kid Pickpenny Exility telemarketer Karl Gauss Homoeomeria contagiously Conglaciation self-critical Diminutival Lozi Maurice Utrillo German Boiling To thrust out Of passage dichloride Circumscriber Besmoked Anacrotic Amiableness Tautaug chiromance Obsceneness Crocalite megavolt quacksalver 1530s En-hakkore FL19 nodi pitsaw Catechumenate C fatuellus pine siskin coop in spear carrier frippet semitone All-murdering punched card H2SO3 disposability blow(1) elastomeric airmail letter fissure of Rolando overreaction Parabola dishonourably ovoidal potentiation featheriness gold piece Ineloquent Tiring polka dots Brugmansia suaveolens salivator Heredia Sevres Dissemination dorsal tentacles elegant with-profits enormous Pretervection myo- Half-heard TVP Unwholesomeness 9F8 lower bound end plate bite off halomorphic Balliage Adenose Tribulation churchless reform Lorain -blastic hearken back turbo go on and on Jochebed Maltworm incognizant stay behind force gondolier viable ACRABBIM emery rock To work off geostrategy testacean dys- dysphoria To hold in pledge ado ribonucleotide Wyrd feudality egoistical CL49 Comely Rancho Palos Verdes Accipiter gentilis CARITES Indemonstrable road pricing homologize SLR Fixed alkalies Pelargonic streptococcal downspout Melanconiaceous Victorville nitrator Xiphoidian Snick and snee bacteriological take aback Unreproached blent Vervels Susters 12AR Panch redistributive FSLIC Marcel Duchamp Merismatic roup(2) deflater Turpitude two dollar bill communist endurance riding stellar wind Francois Rabelais 4IN4 lutein vena iliaca break up caste system Untile lognormality misspend Reproductory Irascibleness go up Palliated PATHRUSIM Royal Navy Villanel trot out unscholarly windtalker Bipinnated cow parsnip Salices sleep rough housecraft blasting cap Amygdalaceae cockcrow Suttner Britching hickory tree Pretendingly -kin Wedlocked anchorless Hyatt RQ Malacothamnus fasciculatus genus Glaux caretaker cardhouse cohune oil Toxicodendron foster-parent To carry on Witfish glycosylation Violin so-and-so Factory Rachel Carson lick one's chops abuzz Mishal swannery coralwort hemolymph sugar candy xenotropic Glorious Diselenide echeveria Soord XS09 IL89 Goodrich wey Rigorous creeper osteogenesis scarlet runner bean pigsny Dr. Johnson motorboating vibist Psalm Lobachevsky Seed gall cell nucleus Protandrous birdbrained inertially glyptography 3AZ8 Q?qon Belaya flunkey MY46 Repprobacy Cessible Finiteness Coachmanship d8Gregarin91 saying Liquor sanguinis semicylinder doodah larkiness chameleon tree frog shareholder sports medicine Libreville resupinate thermochemist dude Air port Pantoscopic Polistes annularis just-in-time manufacturing (JIT) baka dehydrogenate tortfeasor Ailey 62FD fuel injection not intrusive Untangible laser-assisted subepithelial keratomileusis Fraternizing Churchwardenship Moloch Novae spit-and-polish Guadeloupe Tway Faltered CT32 To leave to one's self To give the sack audiogram hack on solid-state national intelligence surveys stimy alliterate S, s pruritus ani zip-code latency phase Albion Interceded Nanga Parbat clitter UR OF THE CHALDEES Sweep-saw second hand laxod orthoboric acid Merestead Leeangle rising prices capital of California over-exercise fictitious character lap of honour Snow flower Desitive European Russia haematogenic genus Salvelinus desacralization WI05 Brachychiton populneus Arthur Holmes weapon of mass destruction NY97 unsure Sicilian Mafia Foreread cerium submarginal adrenocorticotropic jive -ization van(3) Finger post great burdock indissociable quincentennial shortcake Omnigenous Ecballium Marie Byrd Land Logogriphe Poket sporulate hot jazz Pass check peeler alluvial deposit positive pole Jury process spikily Winter Olympic Games Allodially overactive gray lapwing seriously wounded tzar Halibut taqzaw keister balladic smart alecky overweening Cane-hole venturesome hunting Glucinum Schiff wildlife I76 phrasal CH3ONa Colliquated Cholecystis platelet Erciyas booster shot Frazzling Yer babbling warbler Fribbler pound off undisclosable waste matter Disfashion Hurricane- X06 forewent Pretoria-Witwatersrand-Vereeniging canella Alack To promise one's self counterdrug operations tautonym Volcanic ashes Pricksong Illicitly Redemptory Induction Mongolian skeletally Valhalla xanthone Seizing elflock Retrievableness droschke Ellengeness Alloxanate Hierogrammatic 90IA snap brim maximation scrub fowl Moderatorship Anthropologist genus Liatris Cytisus ramentaceus genus Gorgonocephalus Behold Ophism macronucleus Hardly genus Enterobius strippable Toluca de Lerdo carnet CDR Yogacara Co-surety hotel desk clerk megamall repeatable tormentil Jubate Cablet Gallicrex cristatus achromia Strigidae pronounce deaminization Germanic sweets spidery myelocytic CKC electroanalytical monova Dutch florin 75ND Unconceived 0PN3 lobelia Pregnancy facial profiling Uropsilus soricipes Impaneling fat chance somatostatin Bestar filial love half line home from home Baird, John L(ogie) Sorbite epistolatory water tower Demetrius Superscribing mindless Water on the brain fuel-injected dune accountant hieroglyphic gents Rived Monodynamism Hellespont, Hellespontus heroize Hypoptilum Federal Emergency Management Agency Cetology natural logarithm Gemsbok AR95 merchandize Ambreic Columba fasciata 1SD5 old person looney 14J Reckon B'eton Plantago Coronopus Reoviridae take to the woods Cnemidophorus exsanguis Donets Basin bracelet wood Reaccess curtailment Kempis Slackened Tret Chronographer broodingly Stanford-Binet test 5M0 L maculosa trust deed umptieth Lucas (Huyghszoon) van Leyden Drownage Unsighted Subperiosteal O'Neill cannabinol unshared ablism couch potato Lick-spigot Pressirostral sexism swim fin Thomas Pynchon Bone-spavin unforgettable optical cube WS05 Aristocracies opentide TE82 nosologic MI55 Ribeir?o Pr?to To be acknown minicomputer tessellated maple syrup Moor coal L80 Insinewing blue pikeperch Uncompellable Foreworn Corvus frugilegus Swedish mile corroboree Communism Peak Aerie Pharomacrus mocino Ballasting Gusm NY45 Exanguious snapline recollection Promont interparietal genus Gymnopilus AR49 blinkered Divinement Nejdi westmost hopples Scurrilous Allium ascalonicum Adiantum tenerum farleyense Border collie mobile canteen NE90 babul resultant Pediapred hibiscus inductive bye-bye(2) Babylonia pico- Unsin contractible Munched Christmas present cinnamon coloured Cruor Albigenses Phonology Mother water Luxuriously ameer beet leafhopper Peddling -asis Hungarian monetary unit old-man-of-the-woods warning signal clack valve cyanuramide Potshare Bolt and nut maladminister Quail-pipe Loch 1CA1 adventitial poky vase-shaped ghost word Coreus or Anasa tristis fondant fixed-point cabbage-bark tree Needlessness Apostolicism Monasticon grugru nut Bishop, Billy clip(1) spelling straight sinus Affrighten viva Apostle of Germany lobby Moldwarp seed fern dermato- Diacatholicon family Columbidae nuclear deterrence advised isagogics epizoon Reggio Calabria quadriplegia long moss Nilotic enmeshment SIGNS, NUMERICAL Edmund Kean outbuilding 87Y maleic acid Frondesce ASPCA TX16 conversational Ceratonia siliqua Hussite Caterwaul Vimy Ridge flax salivary duct L gigas Antheriferous Protuberancy Scissors grinder resilient Abusing daffily unbreachable wait-list Ojibways tousled Descensory canted justifiedly Droven Angelites immunoreactive Dipterix odorata Diomedea tallgrass prairie Camb. 14OR keno Undercharged mine Excitant Maria Meneghini Callas monetary fund rus in urbe";

            string[] la = lyrics.Split(' ');
            string[] users = { user1ID.UserToken, user2ID.UserToken };
            Random rand = new Random();
            
            for(int i = 0; i < la.Length; i++)
            {
                if(!(la[i] == ""))
                {
                    wordInput.UserToken = users[rand.Next(0, 1)];
                    wordInput.Word = la[i];
                    r = client.DoPutAsync(wordInput, "games/" + gameID).Result;
                    Assert.AreEqual(OK, r.Status);
                }
            }
        }
        [TestMethod]
        public void Test010_LotsOfGames()
        {
            int i = 0;
            while(i < 200)
            {
                dynamic user = new ExpandoObject();
                dynamic gameInput = new ExpandoObject();
                dynamic wordInput = new ExpandoObject();

                user.Nickname = "John";
                Response r = client.DoPostAsync("users", user).Result;
                gameInput.UserToken = r.Data.UserToken;
                gameInput.TimeLimit = 10;
                var user1ID = r.Data;
                r = client.DoPostAsync("games", gameInput).Result;
                var gameID = r.Data.GameID;

                user.Nickname = "Sally";
                r = client.DoPostAsync("users", user).Result;
                gameInput.UserToken = r.Data.UserToken;
                gameInput.TimeLimit = 10;
                var user2ID = r.Data;
                r = client.DoPostAsync("games", gameInput).Result;
                gameID = r.Data.GameID;

                i++;
            }
        }

        //[TestMethod]
        //public void StressTest1()
        //{
        //    int size = 1000;
        //    Dictionary<string, string> users = new Dictionary<string, string>();
        //    Dictionary<string, string> games = new Dictionary<string, string>();
        //    Response r;
        //    dynamic user = new ExpandoObject();
        //    dynamic gameInput = new ExpandoObject();
        //    dynamic wordInput = new ExpandoObject();

        //    for (int i = 0; i < size; i++)
        //    {
        //        user.Nickname = GenerateTokenString(10);
        //        r = client.DoPostAsync("users", user).Result;
        //        users.Add(user.Nickname, r.Data);
        //    }

        //    string token;
        //    for (int i = 0; i < size; i=i+2)
        //    {
        //        gameInput.TimeLimit = 60;
        //        gameInput.UserToken = users.
        //    }

        //    user.Nickname = "@Bob";
        //    r = client.DoPostAsync("users", user).Result;
        //    Assert.AreEqual(Accepted, r.Status);



        //    user.Nickname = "";
        //    r = client.DoPostAsync("users", user).Result;
        //    // bad request
        //    Assert.AreEqual(Forbidden, r.Status);
        //}
    }
}
