using Dadata;
using Dadata.Model;
using System.Configuration;

namespace TestBot
{
    public class InnFinder
    {
        async public Task<string> GetInfoByInn(List<string> innList)
        {
            var listResult = new List<string>();
            foreach (var inn in innList)
            {
                try
                {
                    var keyToken = ConfigurationManager.AppSettings["TokenWeb"].ToString();
                    var keySecret = ConfigurationManager.AppSettings["Secret"].ToString();
                    var keyUri = ConfigurationManager.AppSettings["URI"].ToString();
                    var api = new SuggestClientAsync(keyToken);
                    var response = await api.FindParty(inn);
                    var party = response.suggestions[0].data;
                    var formObject = FormattingObject(party);
                    listResult.Add(formObject);
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                }
            }
            var result = string.Join($"{Environment.NewLine}{Environment.NewLine}", listResult);
            Console.WriteLine($"{result}");
            return result;  
            
        }

        private string FormattingObject(Party party)
        {
            string status;
            if (party.type == PartyType.LEGAL)
                status = "Юридическое лицо";
            else status = "Физическое лицо";

            var qwerty =  $"Полное название: {party.name.full} {Environment.NewLine}" +
                          $"Тип организации: {status}  {Environment.NewLine}" +
                          $"Количество филиалов: {party.branch_count}  {Environment.NewLine}" +
                          $"Адрес: {party.address.data.source}";
            return qwerty;
        }
    }
}
