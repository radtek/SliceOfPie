﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SliceOfPie {
    public class LocalFileModel : IFileModel {
        private string AppPath;
        private string DefaultProjectPath;
        private List<Project> projects = new List<Project>();

        public LocalFileModel() {
            CreateStructure();
            FindProjects();
        }

        public Project AddProject(string title) {
            string projectPath = Path.Combine(AppPath, title);
            if (Directory.Exists(projectPath)) {
                throw new ArgumentException("Project name is already in use (" + projectPath + ")");
            }
            Directory.CreateDirectory(projectPath);
            Project project = new Project();
            project.Title = title;
            project.AppPath = AppPath;
            projects.Add(project);
            return project;
        }

        public void RenameProject(Project project, string title) {
            string projectPath = Path.Combine(AppPath, title);
            if (Directory.Exists(projectPath)) {
                throw new ArgumentException("Project name is already in use (" + projectPath + ")");
            }
            Directory.Move(Path.Combine(AppPath, project.Title), projectPath);
            project.Title = title;
        }

        public override IEnumerable<Project> GetProjects(int userId) {
            foreach (Project project in projects) {
                yield return project;
            }
        }

        public Folder AddFolder(IItemContainer parent, string title) {
            string folderPath = Path.Combine(GetPath("", parent), title);
            if (Directory.Exists(folderPath)) {
                throw new ArgumentException("Project/folder name is already in use (" + folderPath + ")");
            }
            Directory.CreateDirectory(folderPath);
            Folder folder = new Folder();
            folder.Title = title;
            folder.Parent = parent;
            parent.Folders.Add(folder);
            return folder;
        }

        public void RenameFolder(Folder folder, string title) {
            string folderPath = Path.Combine(GetPath("", folder.Parent), title);
            if (Directory.Exists(folderPath)) {
                throw new ArgumentException("Project/folder name is already in use (" + folderPath + ")");
            }
            Directory.Move(GetPath("", folder), folderPath);
            folder.Title = title;
        }

        public Document AddDocument(IItemContainer parent, string title) {
            string documentPath = Path.Combine(GetPath("", parent), title);
            if (File.Exists(documentPath)) {
                throw new ArgumentException("File name is already in use (" + documentPath + ")");
            }
            File.Create(documentPath);
            Document document = new Document();
            document.Title = title;
            document.Parent = parent;
            parent.Documents.Add(document);
            return document;
        }

        public void RenameDocument(Document document, string title) {
            string documentPath = Path.Combine(GetPath("", document.Parent), title);
            if (File.Exists(documentPath)) {
                throw new ArgumentException("File name is already in use(" + documentPath + ")");
            }
            File.Move(Path.Combine(GetPath("", document.Parent), document.Title), documentPath);
            document.Title = title;
        }

        public override void SaveDocument(Document doc) {
            
        }

        public override Document LoadDocument(int docId) {
            return new Document();
        }

        public void CreateStructure() {
            AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SliceOfPie");
            if (!Directory.Exists(AppPath)) {
                Directory.CreateDirectory(AppPath);
            }

            DefaultProjectPath = Path.Combine(AppPath, "default");
            if (!Directory.Exists(DefaultProjectPath)) {
                Directory.CreateDirectory(DefaultProjectPath);
            }
        }

        public void FindProjects() {
            string[] folders = Directory.GetDirectories(AppPath);
            foreach (string folderName in folders) {
                Project project = new Project();
                project.Title = Path.GetFileName(folderName);
                project.AppPath = AppPath;
                projects.Add(project);

                FindFolders(project);
            }
        }

        public void FindFolders(IItemContainer parent) {
            string[] folders = Directory.GetDirectories(GetPath("", parent));
            foreach (string folderName in folders) {
                Folder folder = new Folder();
                folder.Title = Path.GetFileName(folderName);
                folder.Parent = parent;
                parent.Folders.Add(folder);

                FindFolders(folder);
            }
        }

        public void FindDocuments(IItemContainer parent) {
            string[] documentPaths = Directory.GetFiles(GetPath("", parent));
            foreach (string documentName in documentPaths) {
                Document document = new Document();
                document.Title = Path.GetFileName(documentName);
                document.Parent = parent;
                parent.Documents.Add(document);
            }
        }

        public string GetPath(string path, IItemContainer container) {
            if (Directory.Exists(container.AppPath)) {
                return Path.Combine(container.AppPath, Path.Combine(container.Title, path));
            }
            return GetPath(Path.Combine(container.Title, path), container.Parent);
        }
    }
}
