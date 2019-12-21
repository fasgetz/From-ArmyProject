﻿using ArmyClient.LogicApp.Interfaces;
using ArmyClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmyClient.LogicApp.Realisation
{

    /// <summary>
    /// Класс содержит логику работы с БД категории пользователей
    /// </summary>
    internal class UserLogic : IUsersLogic
    {

        private ArmyDBContext db;

        public UserLogic(ArmyDBContext db)
        {
            this.db = db;
        }

        #region Синхронные версии методов

        /// <summary>
        /// Метод по добавлению пользователя в бд
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Возвращает результат добавления</returns>
        public bool AddUser(Users user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion


             #region Асинхронные версии методов

        /// <summary>
        /// Асинхронная версия метод по добавлению пользователя в бд
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> AddUserAsync(Users user)
        {            
            return await Task.Run(() =>
            {
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        
        /// <summary>
        /// Метод получения пользователей
        /// </summary>
        /// <param name="user">Параметр, по модели которой делается выборка</param>
        /// <returns>Возвращает пользователей</returns>
        public async Task<List<Users>> GetUsersAsync(Users user, bool vk = false, bool instagram = false, bool facebook = false)
        {

             
            return await Task.Run(() =>
            {
                var users = from vm in db.Users
                            where
                              (!(string.IsNullOrEmpty(user.Name)) ? vm.Name.Contains(user.Name) : !string.IsNullOrEmpty(vm.Name)) &&
                              (!(string.IsNullOrEmpty(user.Family)) ? vm.Family.Contains(user.Family) : (string.IsNullOrEmpty(vm.Family) || !string.IsNullOrEmpty(vm.Family))) &&
                              (!(string.IsNullOrEmpty(user.Surname)) ? vm.Surname.Contains(user.Surname) : (string.IsNullOrEmpty(vm.Surname) || !string.IsNullOrEmpty(vm.Surname))) &&
                              ((user.DateBirth != null) ? vm.DateBirth == user.DateBirth : (vm.DateBirth >= new DateTime() || vm.DateBirth == null)) &&
                              ((user.IdCountryBirth != null) ? vm.IdCountryBirth == user.IdCountryBirth : vm.IdCountryBirth != 0) &&
                              (!(string.IsNullOrEmpty(user.CityBirth)) ? vm.CityBirth.Contains(user.CityBirth) : (string.IsNullOrEmpty(vm.CityBirth) || !string.IsNullOrEmpty(vm.CityBirth))) &&

                              // Тут самое сложное. По социальным сетям вывести если стоят галочки
                              (((vk == true) ? vm.SocialNetworkUser.FirstOrDefault(i => i.SocialNetworkId == 1).SocialNetworkId == 1 : vm.SocialNetworkUser.FirstOrDefault(i => i.SocialNetworkId == 0).SocialNetworkId == 0) ||
                              ((instagram == true) ? vm.SocialNetworkUser.FirstOrDefault(i => i.SocialNetworkId == 3).SocialNetworkId == 3 : vm.SocialNetworkUser.FirstOrDefault(i => i.SocialNetworkId == 0).SocialNetworkId == 0) ||
                              ((facebook == true) ? vm.SocialNetworkUser.FirstOrDefault(i => i.SocialNetworkId == 2).SocialNetworkId == 2 : vm.SocialNetworkUser.FirstOrDefault(i => i.SocialNetworkId == 0).SocialNetworkId == 0))
                              
                            select vm;

                return users.ToList();

            });
        }


        #endregion


    }
}
