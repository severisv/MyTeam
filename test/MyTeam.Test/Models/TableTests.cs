using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using Xunit;

namespace MyTeam.Test.Models
{
    public class TableTests
    {

        private readonly string _tableString = "Plass   Lag       Kamper   [  Hjemme  ]    [  Borte  ]     [   Total    ]  Diff  Poeng    \neks:\n\n" +
                                                "1	Oslojuvelene	11	4	1	0	24 - 5	6	0	0	19 - 5	10	1	0	43 - 10	33	31 \n" +
                                                "2	Fortuna	        11	4	0	2	18 - 8	4	0	1	19 - 5	8	0	3	37 - 13	24	24         \n" +
                                                "3	Grüner	        11	3	1	1	16 - 9	3	2	1	12 - 9	6	3	2	28 - 18	10	21           \n" +
                                                "4	Asker 2	        11	3	0	2	16 - 15	3	1	2	13 - 8	6	1	4	29 - 23	6	19           \n" +
                                                "5	Høybr/Stovn    11	3	1	1	13 - 10	2	2	2	10 - 11	5	3	3	23 - 21	2	18       \n" +
                                                "6	Heggedal         11	2	3	2	16 - 17	3	0	1	7 - 4	5	3	3	23 - 21	2	18       \n" +
                                                "7	Vollen	        11	2	0	2	9 - 3	        3	2	2	25 - 26	5	2	4	34 - 29	5	17           \n" +
                                                "8	Oldenborg 2    11	1	1	3	6 - 12	2	0	4	16 - 24	3	1	7	22 - 36	-14	10       \n" +
                                                "9	Wam-Kam       10	0	0	6	5 - 27	3	0	1	8 - 8	3	0	7	13 - 35	-22	9            \n" +
                                                "10	Fossum	        11	2	0	4	11 - 16	0	0	5	3 - 19	2	0	9	14 - 35	-21	6            \n" +
                                                "11	Vestli	        11	0	0	6	4 - 13	0	0	5	3 - 19	0	0	11	7 - 32	-25	0            \n" +
                                                "12	Bøkeby	        0	0	0	0	0 - 0	      0	0	0	0 - 0	0	0	0	0 - 0	0	0            \n";

        [Fact]
        public void CreateTable_VerifyCorrectNumberOfLines()
        {
            // Arrange

            // Act
            var table = new Table(Guid.NewGuid(), _tableString);


            // Assert
            Assert.Equal(12, table.Lines.Count);
        }


        [Fact]
        public void CreateTable_VerifyWhiteSpaceInTeamName()
        {
            // Arrange
            var list = new List<string>();


            // Act
            var table = new Table(Guid.NewGuid(), _tableString);

            // Assert
            Assert.Equal("Asker 2", table.Lines[3].Name);
        }
    }
}
