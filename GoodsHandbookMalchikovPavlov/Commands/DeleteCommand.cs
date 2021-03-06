﻿using System;
using System.Diagnostics;
using System.Resources;
using System.Text;
using GoodsHandbookMalchikovPavlov.Properties;

namespace GoodsHandbookMalchikovPavlov.Commands
{
    /// <summary>
    ///     Удаление продукта из списка
    /// </summary>
    internal class DeleteCommand : ICommand
    {
        private readonly IProductCatalog productCatalog;
        private readonly StringBuilder responseBuffer = new StringBuilder();
        private readonly string usage;
        private string[] args;
        private readonly ResourceManager resourceManager = new ResourceManager(typeof(Resources));

        public DeleteCommand(IProductCatalog productCatalog, string[] args)
        {
            usage = resourceManager.GetString("DELETE_USAGE");
            this.productCatalog = productCatalog;
            this.args = args;
        }

        public CommandReturnCode Process(string[] args)
        {
            responseBuffer.Length = 0;
            if (args.Length == 2)
            {
                Debug.Assert(args[0].Equals("delete", StringComparison.OrdinalIgnoreCase));

                int id;
                var isIdValid = int.TryParse(args[1], out id);
                if (isIdValid)
                {
                    isIdValid = productCatalog.IsExist(id);
                    if (isIdValid)
                    {
                        productCatalog.Delete(id);
                        responseBuffer.Append(string.Format("Product with Id = \"{0}\" has been successfully deleted",
                            id));
                        responseBuffer.Append(Environment.NewLine);
                    }
                }

                if (!isIdValid)
                {
                    responseBuffer.Append(string.Format("Product with Id = \"{0}\" does not exist", args[1]));
                    responseBuffer.Append(Environment.NewLine);
                }
            }
            else
            {
                responseBuffer.Append(usage);
            }

            return CommandReturnCode.Done;
        }

        public string GetLastResponse()
        {
            return responseBuffer.ToString();
        }
    }
}