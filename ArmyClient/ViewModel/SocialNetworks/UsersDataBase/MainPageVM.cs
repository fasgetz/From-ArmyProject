﻿using ArmyClient.LogicApp.Helps;
using ArmyClient.Model;
using ArmyClient.View.SocialNetworks._HelpWindows;
using ArmyClient.ViewModel.Helpers;
using ArmyVkAPI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArmyClient.ViewModel.Main
{
    class MainPageVM : MainVM
    {
        #region Свойства        

        // Кнопка поиска юзера
        private bool _SearchUserButtonEnabled = true;
        public bool SearchUserButtonEnabled
        {
            get => _SearchUserButtonEnabled;
            set
            {
                _SearchUserButtonEnabled = value;
                OnPropertyChanged("SearchUserButtonEnabled");
            }
        }

        private ObservableCollection<Model.SoldierUnit> _UserSoldierServices;
        public ObservableCollection<Model.SoldierUnit> UserSoldierServices
        {
            get => _UserSoldierServices;
            set
            {
                _UserSoldierServices = value;
                OnPropertyChanged("UserSoldierServices");
            }
        }

        private Model.Users _SelectedUser;
        public Model.Users SelectedUser
        {
            get => _SelectedUser;
            set
            {
                _SelectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        #region Социальные сети

        private bool _odnoklassniki;
        public bool odnoklassniki
        {
            get => _odnoklassniki;
            set
            {
                _odnoklassniki = value;
                OnPropertyChanged("odnoklassniki");
            }
        }

        private bool _facebook;
        public bool facebook
        {
            get => _facebook;
            set
            {
                _facebook = value;
                OnPropertyChanged("facebook");
            }
        }

        private bool _vk;
        public bool vk
        {
            get => _vk;
            set
            {
                _vk = value;
                OnPropertyChanged("vk");
            }
        }

        private bool _instagram;
        public bool instagram
        {
            get => _instagram;
            set
            {
                _instagram = value;
                OnPropertyChanged("instagram");
            }
        }

        #endregion


        private List<Model.Users> _users;
        public List<Model.Users> users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged("users");
            }
        }


        // Выбранный социальный статус
        private SocialStatuses _SelectedSocStatus;
        public SocialStatuses SelectedSocStatus
        {
            get => _SelectedSocStatus;
            set
            {
                _SelectedSocStatus = value;
                user.SocialStatusID = value.IdStatus;
                user.SocialStatuses = value;
                OnPropertyChanged("SelectedSocStatus");
            }
        }

        // Страны
        private ObservableCollection<SocialStatuses> _SocStatuses;
        public ObservableCollection<SocialStatuses> SocStatuses
        {
            get => _SocStatuses;
            set
            {
                _SocStatuses = value;
                OnPropertyChanged("SocStatuses");
            }
        }

        #region Страны - города

        // Список стран
        private ObservableCollection<Countries> _Countries;
        public ObservableCollection<Countries> Countries
        {
            get => _Countries;
            set
            {
                _Countries = value;
                OnPropertyChanged("Countries");
            }
        }


        // Города выбранной страны проживания
        private ObservableCollection<City> _CitiesResidence;
        public ObservableCollection<City> CitiesResidence
        {
            get => _CitiesResidence;
            set
            {
                _CitiesResidence = value;
                OnPropertyChanged("CitiesResidence");
            }
        }

        #region Секция Военной службы

        // Города выбранной страны в воинских частях
        private ObservableCollection<City> _CitiesUS;
        public ObservableCollection<City> CitiesUS
        {
            get => _CitiesUS;
            set
            {
                _CitiesUS = value;
                OnPropertyChanged("CitiesUS");
            }
        }

        // Выбранная страна воинской части
        private Countries _SelectedCountryUS;
        public Countries SelectedCountryUS
        {
            get => _SelectedCountryUS;
            set
            {
                _SelectedCountryUS = value;

                if (value != null)
                {
                    user.CountryResidence_Id = value.Id;
                    LoadUsCities(value.Id); // Загружаем города страны
                    LoadUnitsCountry(value.Id); // Загружаем В/Ч страны
                }


                //user.City1.CountryId = value.Id;



                OnPropertyChanged("SelectedCountryUS");
            }
        }

        // Выбранный город проживания
        private City _SelectedCityUS;
        public City SelectedCityUS
        {
            get => _SelectedCityUS;
            set
            {
                _SelectedCityUS = value;

                if (value != null)
                    LoadUnitsCity(value.Id);


                OnPropertyChanged("SelectedCityUS");
            }
        }

        #endregion

        // Выбранная страна проживания
        private Countries _SelectedCountryResidence;
        public Countries SelectedCountryResidence
        {
            get => _SelectedCountryResidence;
            set
            {
                _SelectedCountryResidence = value;

                if (value != null)
                {
                    user.CountryResidence_Id = value.Id;
                    LoadResidenceCities(value.Id); // Загружаем города
                }

                OnPropertyChanged("SelectedCountryResidence");
            }
        }

        // Выбранный город проживания
        private City _SelectedCityResidence;
        public City SelectedCityResidence
        {
            get => _SelectedCityResidence;
            set
            {
                _SelectedCityResidence = value;

                if (value != null)
                {                    
                    user.CurrentCityResience_Id = value.Id;

                    // Загружаем В/Ч города
                    LoadUnitsCity(value.Id);
                }
                    
                else
                    user.CurrentCityResience_Id = null;


                OnPropertyChanged("SelectedCityResidence");
            }
        }

        // Выбранный город рождения
        private City _SelectedCityBirth;
        public City SelectedCityBirth
        {
            get => _SelectedCityBirth;
            set
            {
                _SelectedCityBirth = value;

                if (value != null)
                    user.CityBirth_Id = value.Id;
                else
                    user.CityBirth_Id = null;

                OnPropertyChanged("SelectedCityBirth");
            }
        }

        // Города выбранной страны рождения
        private ObservableCollection<City> _CitiesBirthCountry;
        public ObservableCollection<City> CitiesBirthCountry
        {
            get => _CitiesBirthCountry;
            set
            {
                _CitiesBirthCountry = value;
                OnPropertyChanged("CitiesBirthCountry");
            }
        }

        // Выбранная страна рождения
        private Countries _SelectedCountryBirth;
        public Countries SelectedCountryBirth
        {
            get
            {
                return _SelectedCountryBirth;
            }
            set
            {
                _SelectedCountryBirth = value;
                if (value != null)
                {
                    user.CountryBirth_Id = value.Id;                    
                    LoadBirthCities(value.Id);                    
                }
                

                //user.City.CountryId = value.Id;
                OnPropertyChanged("SelectedCountryBirth");
            }
        }


        #endregion


        // Изображение в байтах
        byte[] _ImageBytes;
        public byte[] ImageBytes
        {
            get => _ImageBytes;
            set
            {
                _ImageBytes = value;
                OnPropertyChanged("ImageBytes");
            }
        }

        // Данные пользователя
        private Model.Users _user;
        public Model.Users user
        {
            get => _user;
            set
            {
                _user = value;
                if (user.Photo != null)
                    ImageBytes = user.Photo;
                OnPropertyChanged("user");
            }
        }

        // Веб адрес соц сети
        private string _WebAddress;
        public string WebAddress
        {
            get => _WebAddress;
            set
            {
                _WebAddress = value;
                OnPropertyChanged("WebAddress");
            }
        }

        // Тип соц сети пользователя
        private SocialNetworkType _selectedType;
        public SocialNetworkType SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                OnPropertyChanged("SelectedType");
            }
        }

        // Список соц сетей пользователя
        private ObservableCollection<SocialNetworkUser> _MySocNetTypes;
        public ObservableCollection<SocialNetworkUser> MySocNetTypes
        {
            get => _MySocNetTypes;
            set
            {
                _MySocNetTypes = value;
                OnPropertyChanged("MySocNetTypes");
            }
        }

        // Список соц сетей из БД
        private ObservableCollection<SocialNetworkType> _SocialNetworkTypesList;
        public ObservableCollection<SocialNetworkType> SocialNetworkTypesList
        {
            get => _SocialNetworkTypesList;
            set
            {
                _SocialNetworkTypesList = value;
                OnPropertyChanged("SocialNetworkTypesList");
            }
        }

        // Список В/Ч из БД
        private ObservableCollection<SoldierUnit> _SoldierUnits;
        public ObservableCollection<SoldierUnit> SoldierUnits
        {
            get => _SoldierUnits;
            set
            {
                _SoldierUnits = value;
                OnPropertyChanged("SoldierUnits");
            }
        }

        protected SoldierUnit _SelectedSoldierUnit;
        public SoldierUnit SelectedSoldierUnit
        {
            get => _SelectedSoldierUnit;
            set
            {
                _SelectedSoldierUnit = value;

                if (value == null)
                {
                    user.UserSoldierService = null;
                    return;
                }
                else
                    user.UserSoldierService = new List<UserSoldierService>();

                // Запоминаем старое значение
                if (user.UserSoldierService.Count != 0 && user.UserSoldierService != null)
                {
                    var us = user.UserSoldierService.FirstOrDefault();

                    user.UserSoldierService = new List<UserSoldierService>()
                        {
                            new UserSoldierService(){ Id = us.Id, IdSoldierUnit = value.Id, IdUser = user.Id }
                        };
                }
                else
                {
                    user.UserSoldierService = new List<UserSoldierService>()
                            {
                                new UserSoldierService(){ IdSoldierUnit = value.Id, IdUser = user.Id }
                            };
                }


                OnPropertyChanged("SelectedSoldierUnit");
            }
        }

        private SocialNetworkUser _SelectedSocialNetwork;
        public SocialNetworkUser SelectedSocialNetwork
        {
            get => _SelectedSocialNetwork;
            set
            {
                _SelectedSocialNetwork = value;
                OnPropertyChanged("SelectedSocialNetwork");
            }
        }

        #endregion

        #region Команды

        // Команда по загрузке изображения
        public DelegateCommand LoadImgWeb
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    new LoadImgWebWindow(this).ShowDialog();
                    
                    //Crimes.Add(new Model.UserCrimes() { Id = 123 });

                });
            }
        }


        // Команда по удалению изображения
        public DelegateCommand RemoveImage
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ImageBytes = null;
                    user.Photo = null;
                    //Crimes.Add(new Model.UserCrimes() { Id = 123 });

                });
            }
        }


        // Метод по добавлению изображения
        public DelegateCommand AddImage
        {
            get
            {
                return new DelegateCommand(obj =>
                {

                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Файлы изображений (*.jpg, *.png)|*.jpg;*.png";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        string FilePath = openFileDialog.FileName; // Путь файла изображения

                        ImageBytes = ImageLogic.GetImageBinary(FilePath); // Изображение в бинарном формате
                        user.Photo = ImageBytes;
                    }
                });
            }
        }


        // Команда удаления социальной сети из списка добавленных
        public DelegateCommand RemoveSocNetType
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SelectedSocialNetwork != null)
                    {
                        var item = MySocNetTypes.FirstOrDefault(i => i.GetSocialName == SelectedSocialNetwork.GetSocialName);

                        MySocNetTypes.Remove(item);
                        user.SocialNetworkUser = MySocNetTypes.ToList();
                        SelectedSocialNetwork = null;
                    }
                });
            }
        }

        // Команда парсинга данных из социальной сети
        public DelegateCommand ParseData
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SelectedSocialNetwork != null)
                    {
                        switch (SelectedSocialNetwork.SocialNetworkId)
                        {
                            // Если ВК
                            case (1):
                                LoadDataVK();
                                break;
                            default:
                                break;
                        }
                    }
                });
            }
        }

        // Флаг загрузки
        private bool loading = false;

        private async void LoadDataVK()
        {
            if (loading == false)
            {
                try
                {

                    VkNet.Model.User vk_user = new VkNet.Model.User();
                    await Task.Run(() =>
                    {
                        loading = true;
                        MyApiVK api = new MyApiVK();
                        api.Authorization("89114876557", "Simplepass19");

                        // Получаем айди
                        int id;
                        int.TryParse(string.Join("", SelectedSocialNetwork.WebAddress.Where(c => char.IsDigit(c))), out id);

                        // Если айди == 0, то не удалось спарсить айди с адреса и надо выдать экзепшен
                        if (id == 0)
                            throw new Exception("Введите айди пользователя ВК!");


                        // Получаем пользователя вк по айди
                        vk_user = api.UserLogic.GetUser(id);

                        var socialnetworksuser = user.SocialNetworkUser;

                        // Далее присваиваем значения
                        user = new Model.Users()
                        {
                            Name = vk_user.FirstName,
                            Family = vk_user.LastName,
                            DateBirth = Convert.ToDateTime(vk_user.BirthDate),
                            SocialNetworkUser = socialnetworksuser,
                            IsMonitoring = false
                        };




                        // Далее вбиваем страну, если она есть
                        if (vk_user.Country != null)
                        {
                            SelectedCountryBirth = Countries.FirstOrDefault(i => i.Id == vk_user.Country.Id);
                            SelectedCountryResidence = Countries.FirstOrDefault(i => i.Id == vk_user.Country.Id);
                        }


                        ImageBytes = LoadImage(vk_user.PhotoMaxOrig?.AbsoluteUri);
                        if (ImageBytes != null)
                            user.Photo = ImageBytes;

                    });

                    // Далее необходимо вбить города если он есть у пользователя
                    if (vk_user.City != null)
                    {

                        SelectedCityBirth = CitiesBirthCountry.FirstOrDefault(i => i.Name == vk_user.City.Title);
                        SelectedCityResidence = CitiesResidence.FirstOrDefault(i => i.Name == vk_user.City.Title);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    
                }
                finally
                {
                    loading = false;
                }


            }
            
            
        }


        // Загрузка изображения
        internal byte[] LoadImage(string url)
        {
            using (var webclient = new WebClient())
            {
                try
                {
                    byte[] imageData = null;
                    if (url != null)
                        imageData = webclient.DownloadData(url);

                    return imageData;
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка загрузки");
                    return null;
                }
            }
        }

        // Команда добавления службы пользователю
        public DelegateCommand AddSoldierService
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SelectedSoldierUnit != null)
                    {
                        UserSoldierService service = new UserSoldierService()
                        {
                            IdSoldierUnit = SelectedSoldierUnit.Id,
                            IdUser = user.Id,
                            Id = SelectedSoldierUnit.Id
                        };
                        //service.IdSoldierUnit = SelectedSoldierUnit.Id;
                        //service.IdUser = user.Id;
                        //service.SoldierUnit = SelectedSoldierUnit;
                        user.UserSoldierService.Add(service);
                        UserSoldierServices.Add(SoldierUnits.Where(i => i.Id == service.IdSoldierUnit).FirstOrDefault());
                        SoldierUnits.Remove(SoldierUnits.Where(i => i.Id == service.IdSoldierUnit).FirstOrDefault());

                        SelectedSoldierUnit = null;
                    }
                });
            }
        }


        // Команда по переходу на страницу добавить пользователя

        public DelegateCommand GoToAboutUserPage
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SelectedUser != null)
                        MyNavigation.GoToAboutUser(SelectedUser.Id);
                });
            }
        }

        public DelegateCommand GoToAddUserPage
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    MyNavigation.GoAddUserPage();
                });
            }
        }

        // Команда найти пользователей
        public DelegateCommand SearchUsers
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    //SearchUserButtonEnabled = false;
                    LoadUsers();
                    //SearchUserButtonEnabled = true;
                });
            }
        }

        #endregion

        public MainPageVM()
        {
            // Выделяем память
            //logic = new LogicApp.Realisation.LogicApp();
            //SelectedType = new SocialNetworkType();
            MySocNetTypes = new ObservableCollection<SocialNetworkUser>();
            user = new Model.Users() { City1 = new City(), City = new City() };
            user.IsMonitoring = false;
            UserSoldierServices = new ObservableCollection<SoldierUnit>();

            // Загружаем данные с БД
            LoadData();
        }

        public MainPageVM(bool WatchMod)
            :this()
        {
            vk = true;
            instagram = true;
            facebook = true;
            odnoklassniki = true;
        }

        /// <summary>
        /// Конструктор, который прогружает одного юзера
        /// </summary>
        /// <param name="UserID"></param>
        public MainPageVM(int UserID)
            :this()
        {

        }

        #region Вспомогательные методы 

        // Загрузка городов страны рождения
        protected async void LoadBirthCities(byte idCountry)
        {
            CitiesBirthCountry = new ObservableCollection<City>(await logic.citiesLogic.GetCities(idCountry));
        }

        // Загрузка городов страны проживания
        protected async void LoadResidenceCities(byte idCountry)
        {
            CitiesResidence = new ObservableCollection<City>(await logic.citiesLogic.GetCities(idCountry));
        }

        // Загрузка городов В/Ч
        protected async void LoadUsCities(byte idCountry)
        {
            CitiesUS = new ObservableCollection<City>(await logic.citiesLogic.GetCities(idCountry));
        }

        private async void LoadUsers()
        {
            SearchUserButtonEnabled = false;
            users = await logic.userLogic.GetUsersAsync(user, vk, instagram, facebook, odnoklassniki);

            SearchUserButtonEnabled = true;
        }


        // Вспомогательный метод для загрузки данных
        private async void LoadData()
        {
            SocialNetworkTypesList = new ObservableCollection<SocialNetworkType>(await logic.socialNetworksLogic.LoadSocialNetworkTypesAsync());
            Countries = new ObservableCollection<Countries>(await logic.CountriesLogic.GetCountries());
            SocStatuses = new ObservableCollection<SocialStatuses>(await logic.SocStatusesLogic.GetSocialStatuses());
        }

        // Метод для загрузки В/Ч по городу
        protected async void LoadUnitsCity(int id)
        {
            SoldierUnits = new ObservableCollection<SoldierUnit>(await logic.SoldierUnitLogic.GetSoldierUnitsCityAsync(id));
        }

        // Метод для загрузки В/Ч по стране
        protected async void LoadUnitsCountry(int id)
        {
            SoldierUnits = new ObservableCollection<SoldierUnit>(await logic.SoldierUnitLogic.GetSoldierUnitsCountryAsync(id));
        }

        #endregion


    }
}
