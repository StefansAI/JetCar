// ================================================
//
// SPDX-FileCopyrightText: 2020 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace ImageSegmenter
{
    /// <summary>
    /// The session class handles the navigation in a set of input images and the  
    /// </summary>
    public class Session
    {
        #region Private Fields
        /// <summary>Default file name for the session XML file.</summary>
        private const string FILE_NAME = "SessionInfo.xml";
        /// <summary>Full path to the folder containing all source images.</summary>
        private string pathToSourceImages;
        /// <summary>Full path to the folder for all session related files.</summary>
        private string pathToSessionData;
        /// <summary>List file names still remaining in the list of not processed, i.e. above the currentImageIdx. The borders between processed and remaining were blurred, when free movement back and forth was added.</summary>
        private List<string> remainingFiles;
        /// <summary>List of file names already processed, i.e. below the currentImageIdx. The borders between processed and remaining were blurred, when free movement back and forth was added.</summary>
        private List<string> processedFiles;
        /// <summary>File name of the image currently selected by the currentImageIdx.</summary>
        private string currentImageFileName;
        /// <summary>Index of the image currently selected.</summary>
        private int currentImageIdx;
        #endregion Private Fields

        /// <summary>Delegate definition for passing on one of the load functions.</summary>
        public delegate Bitmap LoadBitmapFunction();

        #region Constructor
        /// <summary>
        /// Creates an instance of the Session class to navgate in the set of input images.
        /// </summary>
        /// <param name="PathToSourceImages">Full path to the folder containing all source images.</param>
        /// <param name="PathToSessionData">Full path to the folder for all session related files.</param>
        public Session(string PathToSourceImages, string PathToSessionData)
        {
            this.pathToSourceImages = PathToSourceImages;
            this.pathToSessionData = PathToSessionData;

            currentImageFileName = "";
            remainingFiles = new List<string>();
            processedFiles = new List<string>();
            currentImageIdx = 0;

            if (File.Exists(SessionFileName))
                LoadSessionInfo();
            else
                SaveSessionInfo();

            string[] fnames = Directory.GetFiles(pathToSourceImages,"*.jpg");
            for (int i = 0; i < fnames.Length; i++)
            {
                string fnameWithoutExtension = Path.GetFileNameWithoutExtension(fnames[i]);
                if (AlreadyProcessed(fnameWithoutExtension) == false)
                    remainingFiles.Add(fnameWithoutExtension);
            }
        }
        #endregion Constructor

        #region Private Methods and Properties
        /// <summary>
        /// Internally used property to get the full path and file name string for the session XML file.
        /// </summary>
        private string SessionFileName
        {
            get { return pathToSessionData + FILE_NAME;  }
        }

        /// <summary>
        /// Returns the full path and file name including added jpg extension for the passed file name only
        /// </summary>
        /// <param name="FileNameWithoutExtension">Plain file name without path and extension.</param>
        /// <returns>Full path and file name with added jpg extension.</returns>
        private string GetImageFileName(string FileNameWithoutExtension)
        {
            return pathToSourceImages + FileNameWithoutExtension + ".jpg";
        }

        /// <summary>
        /// Returns true, if the passed file name exists in the processedFiles list.
        /// </summary>
        /// <param name="FileNameWithoutExtension">Plain file name without path and extension.</param>
        /// <returns>True, if existed in processdFiles</returns>
        private bool AlreadyProcessed(string FileNameWithoutExtension)
        {
            for (int i = 0; i < processedFiles.Count; i++)
                if (processedFiles[i] == FileNameWithoutExtension)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true, if the passed file name exists in the remainingFiles list.
        /// </summary>
        /// <param name="FileNameWithoutExtension"></param>
        /// <returns></returns>
        private bool InRemaining(string FileNameWithoutExtension)
        {
            for (int i = 0; i < remainingFiles.Count; i++)
                if (remainingFiles[i] == FileNameWithoutExtension)
                    return true;

            return false;
        }

        /// <summary>
        /// If currentImageFileName contains a valid file name, this function returns the reference of the loaded Bitmap object.
        /// </summary>
        /// <returns>Bitmap object loaded from the file.</returns>
        private Bitmap LoadCurrentImageFile()
        {
            if (currentImageFileName != null)
            {
                try
                {
                    return (Bitmap)Image.FromFile(GetImageFileName(currentImageFileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading Image File " + currentImageFileName);
                }
            }
            return null;
        }
        #endregion Private Methods and Properties

        #region Public Methods
        /// <summary>
        /// Loads the session information from an XML file via SessionFileName.
        /// It loads currentImageIdx and the plain file names without path and extension into the processedFiles list.
        /// </summary>
        public void LoadSessionInfo()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(SessionFileName);
                XmlNode nodeSession = doc.SelectSingleNode("session");
                currentImageIdx = Convert.ToInt32(nodeSession.SelectSingleNode("current_image_idx").InnerText);

                XmlNode nodeProcessedFiles = nodeSession.SelectSingleNode("processed_files");
                XmlNodeList fileItems = nodeProcessedFiles.SelectNodes("item");
                processedFiles = new List<string>();
                for (int i = 0; i < fileItems.Count; i++)
                    processedFiles.Add(fileItems[i].InnerText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error when loading Settings");
            }
        }

        /// <summary>
        /// Saves the session information to an XML file via SessionFileName.
        /// It saves the currentImageIdx and all plain file names without path and extensions of the processedFiles list.
        /// </summary>
        public void SaveSessionInfo()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlNode nodeSession = doc.AppendChild(doc.CreateElement("session"));
                nodeSession.AppendChild(doc.CreateElement("current_image_idx")).AppendChild(doc.CreateTextNode(currentImageIdx.ToString()));

                XmlNode nodeProcessedFiles = nodeSession.AppendChild(doc.CreateElement("processed_files"));
                foreach (string name in processedFiles)
                    nodeProcessedFiles.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(name));

                doc.Save(SessionFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error when saving Settings");
            }

        }

        /// <summary>
        /// Returns the reference to the Bitmap object loaded from the currentImageIdx in processedFiles list or null if the index is out of bounds.
        /// </summary>
        /// <returns>Reference to Bitmap object loaded.</returns>
        public Bitmap GetCurrent()
        {
            Bitmap bmResult = null;
            currentImageFileName = null;
            if (currentImageIdx <= processedFiles.Count - 1)
            {
                currentImageFileName = processedFiles[currentImageIdx];
                try
                {
                    bmResult = (Bitmap)Image.FromFile(GetImageFileName(currentImageFileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading Image File " + currentImageFileName);
                }
            }
            return bmResult;
        }

        /// <summary>
        /// Increments currentImageIdx and updates currentImageFileName from processedFiles list if available or from remainingFiles list. 
        /// Any previous file name in currentImageFileName not already existing in processedFiles list, will be added and the state stored via SaveSessionInfo().
        /// In that case, the next file name is taken from the remainingFiles list and removed there. In the end this method returns the reference to the Bitmap 
        /// object loaded from currentImageFileName.
        /// </summary>
        /// <returns>Reference to Bitmap object loaded or null if not possible.</returns>
        public Bitmap GetNext()
        {
            if (++currentImageIdx <= processedFiles.Count - 1)
            {
                currentImageFileName = processedFiles[currentImageIdx];
            }
            else
            {
                if (File.Exists(GetImageFileName(currentImageFileName)) == true)
                {
                    if (AlreadyProcessed(currentImageFileName) == false)
                    {
                        processedFiles.Add(currentImageFileName);
                        SaveSessionInfo();
                    }
                }

                currentImageFileName = null;
                if (remainingFiles.Count > 0)
                {
                    currentImageFileName = remainingFiles[0];
                    remainingFiles.RemoveAt(0);
                    if (remainingFiles.Count == 0)
                    {
                        if (AlreadyProcessed(currentImageFileName) == false)
                        {
                            processedFiles.Add(currentImageFileName);
                            currentImageIdx = processedFiles.Count - 1;
                            SaveSessionInfo();
                        }
                    }
                }
                else
                    currentImageIdx = processedFiles.Count - 1;
            }

            return LoadCurrentImageFile();
        }

        /// <summary>
        /// Decreases the currentImageIdx and loads currentImageFileName with a previous file name from processedFiles list if possible.
        /// </summary>
        /// <returns>Reference to Bitmap object loaded or null if not possible.</returns>
        public Bitmap GetPrevious()
        {
            if (currentImageIdx > 0) 
            {
                currentImageIdx--;
                if (currentImageIdx < processedFiles.Count)
                {
                    currentImageFileName = processedFiles[currentImageIdx];
                    SaveSessionInfo();
                }
                return LoadCurrentImageFile();
            }
            return null;
        }

        /// <summary>
        /// Removes the last entry in the processedFiles list and adds it to the remainingFiles list as undoing the processed status.
        /// The file name is however loaded into currentImageFileName and the Bitmap will be loaded from file and returned if possible.
        /// </summary>
        /// <returns>Reference to Bitmap object loaded or null if not possible.</returns>
        public Bitmap RemoveLastProcessed()
        {
            if (File.Exists(GetImageFileName(currentImageFileName)) == true)
            {
                if (InRemaining(currentImageFileName) == false)
                {
                    remainingFiles.Insert(0, currentImageFileName);
                }
            }

            if (processedFiles.Count > 0)
            {
                currentImageFileName = processedFiles[processedFiles.Count - 1];
                processedFiles.RemoveAt(processedFiles.Count - 1);
                SaveSessionInfo();
                return LoadCurrentImageFile();
             }
            return null;
        }

        /// <summary>
        /// Loads the image from file, if the passed index is in the processedFiles bounds and returns the reference to that Bitmap object.
        /// </summary>
        /// <param name="Idx">Index of the file name in processedFiles</param>
        /// <returns>Reference to Bitmap object loaded or null if not possible.</returns>
        public Bitmap GetProcessedImage(int Idx)
        {
            if ((Idx >= 0) && (Idx < processedFiles.Count))
            {
                currentImageFileName = processedFiles[Idx];
                return LoadCurrentImageFile();
            }
            return null;
        }
        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets the plain file name without path or extension of the currently selected image file.
        /// </summary>
        public string CurrentImageFileName
        {
            get { return currentImageFileName; }
        }

        /// <summary>
        /// Gets the full path and file name with extension of the currently selected image file.
        /// </summary>
        public string CurrentImageFileNameFull
        {
            get { return GetImageFileName(currentImageFileName); }
        }

        /// <summary>
        /// Gets the number of file names in the remainingFiles list.
        /// </summary>
        public int RemainingCount
        {
            get { return remainingFiles.Count; }
        }

        /// <summary>
        /// Gets the number of file names in the processedFiles list.
        /// </summary>
        public int ProcessedCount
        {
            get { return processedFiles.Count; }
        }

        /// <summary>
        /// Gets or sets the index for the current image in the processedFiles list bounds.
        /// </summary>
        public int CurrentImageIdx
        {
            get { return currentImageIdx; }
            set
            {
                currentImageIdx = Math.Min(Math.Max(value, -1), processedFiles.Count - 1);
            }
        }
        #endregion Public Properties

    }
}
