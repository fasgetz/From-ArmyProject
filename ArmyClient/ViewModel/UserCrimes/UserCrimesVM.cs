﻿using ArmyClient.LogicApp.Helps;
using ArmyClient.Model;
using ArmyClient.ViewModel.Helpers;
using ArmyClient.ViewModel.Users;
using ArmyVkAPI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model;

namespace ArmyClient.ViewModel.UserCrimes
{

    class UserCrimesVM : AboutUserPageVM
    {

        #region Свойства

        // Апи для работы с ВК
        private MyApiVK api;

        // Выбранный крайм в списке
        private UserCrimesCategory _SelectedCrimeOnLV;
        public UserCrimesCategory SelectedCrimeOnLV
        {
            get => _SelectedCrimeOnLV;
            set
            {
                _SelectedCrimeOnLV = value;
                OnPropertyChanged("SelectedCrimeOnLV");
            }
        }

        // Для отображения в списке
        private ObservableCollection<UserCrimesCategory> _MyCrimesCategory;
        public ObservableCollection<UserCrimesCategory> MyCrimesCategory
        {
            get => _MyCrimesCategory;
            set
            {
                _MyCrimesCategory = value;
                OnPropertyChanged("MyCrimesCategory");
            }
        }

        private CrimesType _SelectedCategory;
        public CrimesType SelectedCategory
        {
            get => _SelectedCategory;
            set
            {
                _SelectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }

        // Выбранная социальная сеть
        private SocialNetworkUser _selectedSocialNetwork;
        public SocialNetworkUser selectedSocialNetwork
        {
            get => _selectedSocialNetwork;
            set
            {
                _selectedSocialNetwork = value;
                OnPropertyChanged("selectedSocialNetwork");
            }
        }

        // Преступление
        private Model.UserCrimes _Crime;
        public Model.UserCrimes Crime
        {
            get => _Crime;
            set
            {
                _Crime = value;
                //ImageBytes = null;
                OnPropertyChanged("Crime");
            }
        }


        // Преступление
        private Model.UserCrimes _MyCrime;
        public Model.UserCrimes MyCrime
        {
            get => _MyCrime;
            set
            {
                _MyCrime = value;
                Crime = value;
                if (value != null)
                {
                    ImageBytes = value.Photo;

                    MyCrimesCategory = new ObservableCollection<UserCrimesCategory>(value.UserCrimesCategory);
                    CrimesCategory = new ObservableCollection<CrimesType>();

                    // Теперь необходимо выбрать элементы, которые еще можно добавить в список
                    var list = new List<CrimesType>();

                    // Перебираем коллекцию
                    foreach (var item in CrimesTypesAll)
                    {
                        // Проверяем. Есть ли элемент в коллекции
                        var type = MyCrimesCategory.FirstOrDefault(i => i.CrimesTypeId == item.Id);

                        // Если есть, то удаляем
                        if (type == null)
                        {
                            list.Add(item);
                            CrimesCategory.Add(item);
                        }
                    }
                }
                OnPropertyChanged("MyCrime");
            }
        }

        // Преступления пользователя
        private ObservableCollection<Model.UserCrimes> _Crimes;
        public ObservableCollection<Model.UserCrimes> Crimes
        {
            get => _Crimes;
            set
            {
                _Crimes = value;
                OnPropertyChanged("Crimes");
            }
        }


        // Общие Типы преступлений
        private ObservableCollection<CrimesType> _CrimesCategory;
        public ObservableCollection<CrimesType> CrimesCategory
        {
            get => _CrimesCategory;
            set
            {
                _CrimesCategory = value;
                OnPropertyChanged("CrimesCategory");
            }
        }

        // ВСЕ типы преступлений, которые могут быть
        private List<CrimesType> _CrimesTypesAll;
        public List<CrimesType> CrimesTypesAll
        {
            get => _CrimesTypesAll;
            set
            {
                _CrimesTypesAll = value;
                OnPropertyChanged("CrimesTypesAll");
            }
        }

        #endregion

        #region Вспомогательные методы

        private async void LoadData()
        {
            if (CrimesTypesAll == null)
                CrimesTypesAll = await logic.CrimesLogic.LoadCrimesCategory();

            Crimes = new ObservableCollection<Model.UserCrimes>(await logic.CrimesLogic.GetSocialNetworkCrimes(selectedSocialNetwork.Id));
        }

        private async void UpdateCrime()
        {
            bool updated = await logic.CrimesLogic.EditCrime(Crime);

            // Если успешно, то прогрузи
            if (updated == true)
                LoadData();


            ImageBytes = null;
            Crime = null;
            MyCrime = null;
            MyCrimesCategory = null;
            CrimesCategory = null;
        }


        private async void AddCrimeDB()
        {

            // Добавляем преступление в БД
            bool added = await logic.CrimesLogic.AddCrime(Crime);

            // Если успешно, то прогрузи
            if (added == true)
                LoadData();


            ImageBytes = null;
            Crime = null;
            MyCrime = null;
        }

        #endregion

        #region Команды

        // Команда по удалению категории из списка
        public DelegateCommand RemoveCrimesCategory
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SelectedCrimeOnLV != null)
                    {
                        var item = SelectedCrimeOnLV;
                        MyCrimesCategory.Remove(item);
                        CrimesCategory.Add(item.CrimesType);                        
                        Crime.UserCrimesCategory.Remove(Crime.UserCrimesCategory.FirstOrDefault(i => i.CrimesTypeId == item.CrimesTypeId));

                        SelectedCategory = null;
                    }
                });
            }
        }


        // Команда по добавлению категории в список
        public DelegateCommand AddCategory
        {
            get
            {
                return new DelegateCommand(obj =>
                {


                    if (SelectedCategory != null)
                    {
                        var item = SelectedCategory;

                        MyCrimesCategory.Add(new UserCrimesCategory() { CrimesTypeId = item.Id, CrimesType = item });
                        CrimesCategory.Remove(SelectedCategory);
                        Crime.UserCrimesCategory.Add(new UserCrimesCategory() { CrimesTypeId = item.Id });
                        //Crime.UserCrimesCategory.Add(new UserCrimesCategory() { CrimesTypeId = SelectedCategory.Id });

                        SelectedCategory = null;
                    }
                  
                });
            }
        }

        // Команда по добавлению изображения нарушению
        public DelegateCommand AddCrimeImage
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (Crime != null)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Файлы изображений (*.jpg, *.png)|*.jpg;*.png";

                        if (openFileDialog.ShowDialog() == true)
                        {
                            string FilePath = openFileDialog.FileName; // Путь файла изображения

                            ImageBytes = ImageLogic.GetImageBinary(FilePath); // Изображение в бинарном формате
                            Crime.Photo = ImageBytes;
                        }
                    }
                });
            }
        }

        // Команда по добавлению изображения нарушению
        public DelegateCommand RemoveCrimeImage
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ImageBytes = null;
                    Crime.Photo = ImageBytes;
                });
            }
        }

        // Команда по добавлению нарушения
        public DelegateCommand AddCrime
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Crime = new Model.UserCrimes()
                    {
                        DateEnty = DateTime.Now,
                        IdSocialNetworkUser = selectedSocialNetwork.Id
                    };

                    //MyCrime = null;
                    ImageBytes = null;
                });
            }
        }

        // Команда по сохранению нарушения
        public DelegateCommand SaveCrime
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (Crime != null)
                    {
                        // Добавляем преступление в БД, если айди == 0, т.к. объект только создан
                        if (Crime.Id == 0)
                            AddCrimeDB();

                        // Иначе обновляем объект в БД
                        UpdateCrime();
                    }

                });
            }
        }


        #endregion
        
        public UserCrimesVM(Model.Users user, SocialNetworkUser selectedSocialNetwork)
        {
            this.user = user;
            this.selectedSocialNetwork = selectedSocialNetwork;            
            ImageBytes = null;
            MyCrimesCategory = new ObservableCollection<UserCrimesCategory>();

            // Загружаем данные
            LoadData();


            //api.

        }

        // Команда по автозагрузке иностранных друзей
        public DelegateCommand autoload
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Loads(189251970);
                });
            }
        }

        private async void Loads(int UserID)
        {
            await Task.Run(() =>
            {
                api = new MyApiVK();
                api.Authorization("89114876557", "Simplepass19");

                ForeignFriends = api.UserLogic.GetForeignFriends(UserID).ToList();
            });
        }

        private List<User> _ForeignFriends;
        public List<User> ForeignFriends
        {
            get => _ForeignFriends;
            set
            {
                _ForeignFriends = value;
                OnPropertyChanged("ForeignFriends");
            }
        }

    }
}
