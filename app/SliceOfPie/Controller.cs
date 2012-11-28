﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    public class Controller {
        private static Controller _instance;

        private FileModel fileModel;

        public Controller() {
            fileModel = new LocalFileModel();
        }

        /// <summary>
        /// Get An IEnumerable of the project associated with the user denoted by userId
        /// (defaults to User.Local as a convenience for the local client)
        /// </summary>
        /// <returns>returns project related to user.</returns>
        public IEnumerable<Project> GetProjects(int userId = 42) {
            foreach (Project p in fileModel.GetProjects(userId)) {
                yield return p;
            }
        }
    }
}