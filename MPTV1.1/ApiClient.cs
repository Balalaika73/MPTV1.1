using College_API_V1.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging.Abstractions;
using MPTV1._1.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Macs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MPTV1._1
{
    internal class ApiClient
    {
        private readonly string apiUrl;


        public ApiClient(string apiUrl)
        {
            this.apiUrl = apiUrl;
        }

        public async Task<string> AuthenticateAsync(User user)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}/Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Обработка ошибки (можете выбрать подходящий для вас способ)
                    return null;
                }
            }
        }

        public async Task<string> RegisterUserAsync(UserRegistrationRequest user)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}/Auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return errorMessage;
                }
            }
        }

        public async Task<string> PostDirectionEmployee(DirectionEmployee dirEmpl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(dirEmpl), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}/dirEmpls", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Обработка ошибки (можете выбрать подходящий для вас способ)
                    return null;
                }
            }
        }

        public async Task<string> UpdateAbiturientSNILSAsync(byte[] SNILSphoto)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var SNILS = new
                {
                    SNILSPhoto = SNILSphoto
                };
                var content = new StringContent(JsonConvert.SerializeObject(SNILS), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync($"{apiUrl}/Abiturients/{App.Token}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return errorMessage;
                }
            }
        }

        public async Task<string> UpdateAbiturientPassportAsync(byte[] Passportphoto)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var Passport = new
                {
                    passportPhoto = Passportphoto
                };
                var content = new StringContent(JsonConvert.SerializeObject(Passport), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync($"{apiUrl}/Abiturients/{App.Token}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return errorMessage;
                }
            }
        }

        public async Task<string> UpdateAbiturientCertAsync(int cert)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var certification = new
                {
                    certId = cert
                };
                var content = new StringContent(JsonConvert.SerializeObject(certification), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync($"{apiUrl}/Abiturients/{App.Token}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return errorMessage;
                }
            }
        }





        public async Task<string> UpdateAbiturientAsync(abiturient abiturient)
        {
            using (HttpClient httpClient = new HttpClient())
            {

                var content = new StringContent(JsonConvert.SerializeObject(abiturient), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync($"{apiUrl}/Abiturients/{App.Token}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return errorMessage;
                }
            }
        }

        public async Task<bool> CheckIfStatementExists(string encryptedGmail, string direction)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{apiUrl}/abStatements/ExistsByGmailDirection?encryptedGmail={encryptedGmail}&direction={direction}";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    bool exists = await response.Content.ReadAsAsync<bool>();
                    return exists;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return false;
                }
            }
        }

        public async Task<int> PostPersAchieveAsync(persAchiefe persAchieve)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Проверяем наличие записи перед выполнением операции
                    var existingAchieveId = await GetPersAchieveByEmailAsync(App.Token);

                    // Если запись существует, выполняем операцию обновления (PUT)
                    if (existingAchieveId != 0)
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(persAchieve), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await httpClient.PutAsync($"{apiUrl}/persAchieves/{existingAchieveId}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadAsAsync<int>();
                        }
                        else
                        {
                            // Обработка ошибки (можете выбрать подходящий для вас способ)
                            return -1;
                        }
                    }
                    // Если запись отсутствует, выполняем операцию создания новой записи (POST)
                    else
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(persAchieve), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}/persAchiefes", content);

                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            return int.TryParse(result, out int parsedResult) ? parsedResult : -1;
                        }
                        else
                        {
                            // Обработка ошибки (можете выбрать подходящий для вас способ)
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения
                return -1;
            }
        }

        public async Task<int> GetPersAchieveByEmailAsync(string userEmail)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"{apiUrl}/persAchiefes/GetPersAchieveByEmail?userEmail={userEmail}");

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        // Преобразовываем строку в число (achieveId)
                        return int.Parse(result);
                    }
                    else
                    {
                        // Обработка ошибки (можете выбрать подходящий для вас способ)
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения
                return 0;
            }
        }

        public async Task<List<mptGroup>> GetMptGroupsAsync(string directionName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{apiUrl}/mptGroups/GetGroupsByDirection/{directionName}");

                if (response.IsSuccessStatusCode)
                {
                    var groupsJson = await response.Content.ReadAsStringAsync();
                    var groups = JsonConvert.DeserializeObject<List<mptGroup>>(groupsJson);
                    List<string> groupNumb = groups.Select(r => r.grNumb).ToList();
                    return groups;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<direction>> GetDirectionsAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{apiUrl}/directions");

                if (response.IsSuccessStatusCode)
                {
                    var directionsJson = await response.Content.ReadAsStringAsync();
                    var directions = JsonConvert.DeserializeObject<List<direction>>(directionsJson);

                    return directions;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<int> UploadCertificateAsync(certificate certificate)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonContent = JsonConvert.SerializeObject(certificate);

                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/certificates", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Ошибка API: {response.ReasonPhrase}");
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        // Получаем URL созданного ресурса
                        string createdResourceUrl = response.Headers.Location.ToString();

                        // Извлекаем ID из URL
                        string[] segments = createdResourceUrl.Split('/');
                        string idString = segments.Last();

                        // Преобразуем строковый ID в целочисленный формат
                        if (int.TryParse(idString, out int id))
                        {
                            // Используйте id по вашему усмотрению
                            return id;
                        }
                        else
                        {
                            throw new Exception($"Ошибка при преобразовании ID в int: {idString}");
                        }
                        return -1;
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при отправке запроса: {ex.Message}");
            }
        }


        public async Task<string> UpdateStatementStatusAsync(string status, string stateNumb)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var stStatus = new
                {
                    stateSt = status
                };
                var content = new StringContent(JsonConvert.SerializeObject(stStatus), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync($"{apiUrl}/abStatements/{stateNumb}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return errorMessage;
                }
            }
        }

        public async Task<string> AddFIOAbAsync(abiturient abiturient)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var fio = new
                    {
                        abSur = abiturient.abSur,
                        abName = abiturient.abName,
                        abSecN = abiturient.abSecN
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(fio), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{apiUrl}/abiturients/{App.Token}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Server error: {errorMessage}");
                        return null; // Обработка ошибки
                    }
                }
            }
            catch (Exception ex)
            {
                return null; // Обработка ошибки
            }
        }

        public async Task<string> AddStatementAsync(abStatement statement, string token)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    statement.numbState = "1";
                    var content = new StringContent(JsonConvert.SerializeObject(statement), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/abStatements", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Server error: {errorMessage}");
                        return null; // Обработка ошибки
                    }
                }
            }
            catch (Exception ex)
            {
                return null; // Обработка ошибки
            }
        }

        public async Task<string> UpdateStatementAsync(string encryptedGmail, string direction, abStatement updatedStatement)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var statement = new
                    {
                        photoState = updatedStatement.photoState,
                        freeEduc = updatedStatement.freeEduc,
                        abGmail = encryptedGmail,
                        stateDir = updatedStatement.stateDir,
                        stateSt = updatedStatement.stateSt
                    };
                    var jsonContent = JsonConvert.SerializeObject(statement);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{apiUrl}/abStatements/UpdateStatement?encryptedGmail={encryptedGmail}&direction={direction}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync(); ;
                    }
                    else
                    {
                        throw new Exception($"Ошибка API: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при отправке запроса: {ex.Message}");
            }
        }


        public async Task<abiturient> GetAbiturientByEmailAsync(string email)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", email);

                    HttpResponseMessage response = await client.GetAsync($"{apiUrl}/abiturients/{email}");

                    if (response.IsSuccessStatusCode)
                    {
                        var abiturientJson = await response.Content.ReadAsStringAsync();
                        var abiturient = JsonConvert.DeserializeObject<abiturient>(abiturientJson);

                        return abiturient;
                    }
                    else
                    {
                        return null; // Обработка ошибки
                    }
                }
            }
            catch (Exception ex)
            {
                return null; // Обработка ошибки
            }
        }

        public async Task<List<DisplayableAbiturient>> GetAbiturientsByStatementsAsync(string directionName)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"{apiUrl}/abStatements/GetAbiturientsByStatements?encryptedDirectionName={directionName}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        List<DisplayableAbiturient> abiturients = JsonConvert.DeserializeObject<List<DisplayableAbiturient>>(responseBody);
                        return abiturients;
                    }
                    else
                    {
                        // Обработка ошибки
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                return null;
            }
        }

        public async Task<List<PCDisplayAbiturients>> GetPCAbiturientsByStatementsAsync(string directionName)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"{apiUrl}/abStatements/GetPCAbiturientsByStatements?encryptedDirectionName={directionName}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        List<PCDisplayAbiturients> abiturients = JsonConvert.DeserializeObject<List<PCDisplayAbiturients>>(responseBody);
                        return abiturients;
                    }
                    else
                    {
                        // Обработка ошибки
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                return null;
            }
        }


        public async Task<AbDisplayAbiturient> GetAbAbiturientsByEmail(string email, string directionName)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"{apiUrl}/abStatements/GetAbAbiturientsByEmail?email={email}&encryptedDirectionName={directionName}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var abiturientDocs = JsonConvert.DeserializeObject<AbDisplayAbiturient>(responseBody);
                        return abiturientDocs;
                    }
                    else
                    {
                        // Обработка ошибки
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                return null;
            }
        }


        public async Task<List<log>> GetLogsAsync()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"{apiUrl}/logs");

                    if (response.IsSuccessStatusCode)
                    {
                        string logsJson = await response.Content.ReadAsStringAsync();
                        List<log> logs = JsonConvert.DeserializeObject<List<log>>(logsJson);
                        return logs;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> PostLogAsync(log logEntry)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(logEntry), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}/logs", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Handle error appropriately
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                return null;
            }
        }


        public async Task<string> PostStudentAsync(student student)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(student), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}/students", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Handle error appropriately
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                return null;
            }
        }


        public async Task<string> PostStudentToGroupAsync(stGroup stGroup)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(stGroup), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync($"{apiUrl}/stGroups", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Handle error appropriately
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                return null;
            }
        }


        public async Task DownloadPDF(string filePath)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"{apiUrl}/Pdf/Download");

                    if (response.IsSuccessStatusCode)
                    {
                        // Получаем байты файла
                        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                        System.IO.File.WriteAllBytes(filePath, fileBytes);

                        MessageBox.Show($"Файл успешно скачан и сохранен по пути: {filePath}");
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при скачивании файла: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }


        public async Task<byte[]> SendDocAsync()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"{apiUrl}/Pdf/Send");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<abiturientModel> GetAbiturientByStatement(string stateNumb)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"{apiUrl}/abStatements/GetAbiturientByStatement?stateNumb={stateNumb}");

                    if (response.IsSuccessStatusCode)
                    {
                        var abiturientJson = await response.Content.ReadAsStringAsync();
                        var abiturient = JsonConvert.DeserializeObject<abiturientModel>(abiturientJson);

                        return abiturient;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
