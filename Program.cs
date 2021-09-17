/*******************************************************
 * Lista Arquivos
 * 
 * Creator: Augusto Lange
 * 
 * Last Change: 17/03/2013
 * Version: 1.20
 *  
 * ****************************************************/
/*****************************************************
 *  Version 1.00:
 *  
 *  Date: 27/07/2011
 * 
 * - Code the Idea;
 * - Search for files in directories and subdirectories.
 * 
 *****************************************************/
/*****************************************************
 *  Version 1.10:
 *  
 *  Date: 12/08/2012
 * 
 * - Added option to hide the extension of the file on the generated list. The default is show.
 * 
 *****************************************************/
/*****************************************************
 *  Version 1.20:
 *  
 *  Date: 17/03/2013
 * 
 * - Added option to generate a list with only path of files.
 * - Added option to search files by date.
 * 
 *****************************************************/
/*****************************************************
 *  Version 1.30:
 *  
 *  Date: 17/03/2013
 * 
 * - Added option to generate a list with full path of files.
 * - Support to multiple file extensions.
 * 
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ListaArquivos
{
	class Program
	{
		private const String APPNAME = "Lista Arquivos";
		private const String AUTHOR = "Augusto Lange";
		private const String VERSION = "1.3";
		private const String COPYRIGHT = "Copyright © 2013";
		private const String FILENAME = "Arquivos.txt";
        private const String FOLDERNAME = ".\\Lista\\";

		private bool SubFolders { get; set; }
		private bool Help { get; set; }
		private bool Pause { get; set; }
		private bool Numeric { get; set; }
        private bool HideExtension { get; set; }
        private bool OnlyPath { get; set; }
        private bool FullPath { get; set; }
        private bool DateSearch { get; set; }
        private int NewerThan { get; set; }
        private String Extension { get; set; }
		private String FileName { get; set; }
		private String PathName { get; set; }
        private String ExtensionName { get; set; }
        private String FileDate { get; set; }
		
		static void Main(string[] args)
		{
			Program p = new Program();
			p.Run(args);
		}

		void Run(string[] args)
		{
			FileName = FILENAME;
			Extension = "*.*";
            PathName = ".\\";

			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-f":
                        ExtensionName = (args[++i]).Trim('*', '.');

                        foreach (String strExtension in ExtensionName.Split('|'))
                        {  
                            if (Extension == "*.*")
                                Extension = "*." + strExtension;
                            else
                                Extension += "|*." + strExtension;
                        }

						break;

					case "-s":
						SubFolders = true;
						break;

					case "-p":
						Pause = true;
						break;

					case "-c":
                        PathName = args[++i];
						break;

					case "-n":
						FileName = args[++i];
						break;

					case "-nr":
						Numeric = true;
						break;

                    case "-he":
                        HideExtension = true;
                        break;

                    case "-op":
                        OnlyPath = true;
                        break;

                    case "-fp":
                        FullPath = true;
                        break;

                    case "-d":
                        DateSearch = true;

                        String arg = args[++i];

                        if (arg.Substring(0, 1) == "+")
                            NewerThan = 1;
                        else if (arg.Substring(0, 1) == "-")
                            NewerThan = -1;
                        else
                            NewerThan = 0;

                        FileDate = arg.Substring(1);

                        break;

					default:
					case "-h":
						Help = true;
						break;
				}
			}

			if (!Help)
			{
				FindFiles();
				Console.WriteLine(Credits());
			}
			else
			{
				ShowHelp();
			}

			if (Pause)
			{
				Console.WriteLine("\nPressione qualquer tecla para continuar...");
				Console.ReadKey();
			}
		}

		private String Credits()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(APPNAME);
			sb.AppendLine("Autor: " + AUTHOR);
			sb.AppendLine("Versão: " + VERSION);
			sb.AppendLine(COPYRIGHT);

			return sb.ToString();
		}

        private String[] SelectFilePaths(String[] files)
        {
            DateTime DesiredDate = Convert.ToDateTime(FileDate);
            FileInfo fi;
            List<String> NewFiles = new List<String>();

            for (int i = 0; i < files.Length; i++)
            {
                fi = new FileInfo(files[i]);

                if (DateTime.Compare(fi.CreationTime, DesiredDate) == NewerThan)
                    NewFiles.Add(fi.FullName);
            }

            return NewFiles.ToArray();
        }

		public void FindFiles()
		{
			try
			{
                if (!Directory.Exists(FOLDERNAME))
                    Directory.CreateDirectory(FOLDERNAME);

                String[] filePaths = GetFiles(PathName, Extension, (SubFolders == true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
				StreamWriter sw = new StreamWriter(FOLDERNAME + FileName, false);
                String strFilePath = String.Empty;

				sw.WriteLine("Lista de arquivos {0}\n", Extension);

                if (DateSearch)
                    filePaths = SelectFilePaths(filePaths);

				for (int i = 0; i < filePaths.Length; i++)
				{
                    if (OnlyPath)
                        strFilePath = Path.GetDirectoryName(filePaths[i]);
                    else if (FullPath)
                        strFilePath = Path.GetFullPath(filePaths[i]);
                    else
                    {
                        if (HideExtension)
                            strFilePath = Path.GetFileNameWithoutExtension((filePaths[i].Split('\\')).Last());
                        else
                            strFilePath = (filePaths[i].Split('\\')).Last();
                    }

                    sw.WriteLine((Numeric == true ? (i + 1).ToString() + " - " : "") + strFilePath);
				}

				sw.WriteLine("\n" + Credits());

				sw.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Erro:\n\t{0}", ex.Message);
			}
		}

        private static String[] GetFiles(String sourceFolder, String filters, System.IO.SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }

		public void ShowHelp()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("Lista Arquivos:");
			sb.AppendLine("\tGera um arquivo texto contendo uma lista dos arquivos solicitados");
			sb.AppendLine("\n\tComandos:");
			sb.AppendLine("\n\t\t-h:");
			sb.AppendLine("\t\t\tMostra esta ajuda;");
			sb.AppendLine("\n\t\t-f <extensão[|extensão]>:");
			sb.AppendLine("\t\t\tExplicita as extensões dos arquivos desejados, caso contrário lista todos os arquivos do(s) diretório(s);");
			sb.AppendLine("\n\t\t-n <nome>:");
			sb.AppendLine("\t\t\tExplicita o nome do arquivo a ser gerado;");
            sb.AppendLine("\n\t\t-c <caminho>:");
            sb.AppendLine("\t\t\tExplicita o caminho para o diretório que encontram-se os arquivos para se gerar a lista;");
			sb.AppendLine("\n\t\t-nr:");
			sb.AppendLine("\t\t\tGera uma lista numerada;");
            sb.AppendLine("\n\t\t-he:");
            sb.AppendLine("\t\t\tMostra a extensão dos arquivos na lista a ser gerada;");
            sb.AppendLine("\n\t\t-op:");
            sb.AppendLine("\t\t\tMostra somente o diretório dos arquivos;");
            sb.AppendLine("\n\t\t-fp:");
            sb.AppendLine("\t\t\tMostra o caminho completo dos arquivos (diretório + arquivo);");
            sb.AppendLine("\n\t\t-d [+|-|]<data>:");
            sb.AppendLine("\t\t\tProcura por arquivos anteriores (-data), posteriores (+data) ou na data indicada (data);");
			sb.AppendLine("\n\t\t-s:");
			sb.AppendLine("\t\t\tPesquisa em subdiretórios;");
			sb.AppendLine("\n\t\t-p:");
			sb.AppendLine("\t\t\tPausa a aplicação ao fim.\n");

			sb.AppendLine(Credits());

			Console.Write(sb.ToString());
		}
	}
}
