﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using GoodsHandbookMalchikovPavlov.Model;
using GoodsHandbookMalchikovPavlov.Validators;
namespace GoodsHandbookMalchikovPavlov
{
    internal sealed class ProductCachedStuff
    {
        private string productName;
        private PropertyInfo[] propertyInfoArray;
        private string[] properPropertyNames;

        public string ProductName { get { return productName; } set { productName = value; } }
        public PropertyInfo[] PropertyInfoArray { get { return propertyInfoArray; } set { propertyInfoArray = value; } }
        public string[] ProperPropertyNames { get { return properPropertyNames; } set { properPropertyNames = value; } }

    }
    internal enum Command
    { None, Create, List}
    public sealed class MainLoop
    {
        private static readonly string USAGE_TEXT =
           "Program can be used to create, edit and view products information\n" +
           "Usage: This is an interactive kind of program. Type any command name listed below\n" +
           "and go through steps it takes to acomplish your task\n" +
           "create [product name] - to create a new record of a given product type\n" +
           "list [product name]   - to list product records, optionally filter by product type\n" +
           "help [command name]   - to get detailed information about a given command if applicable\n";

        private readonly Dictionary<string, Command> commandMap = new Dictionary<string, Command>()
        {
            { "create", Command.Create },
            { "list", Command.List },
        };
        private readonly Dictionary<string, Type> nameToProductTypeMap;
        private readonly Dictionary<Type, ProductCachedStuff> productMap;
        private readonly Dictionary<Type, ProductValidator> validatorMap;

        private List<Product> storage = new List<Product>();

        private bool attemptingToExit = false;

        private Command activeCommand= Command.None;
        private string promptPrefix = ">";

        private StringBuilder inputBuffer = new StringBuilder();
        private StringBuilder outputBuffer = new StringBuilder();
        private bool outputAttention = false;

        private bool interruptedByCtrlCombination;
        private ConsoleKey keyPressedWithCtrl;

        private Type createProductType = null;
        private Product createProduct = null;
        private bool createInputRequested = false;
        private int createPropertyIndex = 0;
        private ProductCachedStuff createProductStuff = null;
        private ProductValidator createValidator = null;

        private Type listProductType = null;
        private bool listInputRequested = false;

        public MainLoop()
        {
            nameToProductTypeMap = new Dictionary<string, Type>();
            validatorMap = new Dictionary<Type, ProductValidator>();
            Type[] productTypes = new Type[]
                {
                    typeof(Toy),
                    typeof(Book),
                    typeof(HomeAppliances)
                };
            ProductValidator[] validators = new ProductValidator[]
                {
                    new ToyValidator(),
                    new BookValidator(),
                    new HomeAppliancesValidator()
                };
            for (int i = 0; i < productTypes.Length; i++)
            {
                var type = productTypes[i];
                nameToProductTypeMap.Add(ReflectionMisc.GetTypeName(type), type);
                validatorMap.Add(type, validators[i]);
            }

            productMap = CreateDictionaryOfCachedProductStuff(productTypes);
        }
        public void Begin()
        {
            outputBuffer.Append(USAGE_TEXT);
            OutputResponse();
            bool isRunning = true;
            while (isRunning)
            {
                OutputPrompt();
                GatherUserInput();
                isRunning = !ProcessUserInput();
                OutputResponse();
            }
        }
        private void GatherUserInput()
        {
            Console.TreatControlCAsInput = true;
            inputBuffer.Length = 0;
            int inputBufferIndex = 0;
            int promptPrefixLength = promptPrefix.Length;
            interruptedByCtrlCombination = false;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                bool printable = !Char.IsControl(keyInfo.KeyChar);

                if (keyInfo.Modifiers == ConsoleModifiers.Control)
                {
                    interruptedByCtrlCombination = true;
                    keyPressedWithCtrl = keyInfo.Key;
                  break;
                }

                else
                {

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                       break;
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (inputBufferIndex > 0)
                        {
                            ClearPromptLine(inputBuffer.Length, promptPrefixLength);

                            inputBufferIndex--;
                            inputBuffer.Remove(inputBufferIndex, 1);

                            Console.Write(inputBuffer.ToString());
                            Console.SetCursorPosition(promptPrefixLength + inputBufferIndex, Console.CursorTop);
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        if (inputBufferIndex > 0)
                        {
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            inputBufferIndex--;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        if (inputBufferIndex < inputBuffer.Length)
                        {
                            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                            inputBufferIndex++;
                        }
                    }
                    else if (printable)
                    {
                        inputBuffer.Insert(inputBufferIndex++, keyInfo.KeyChar);
                        ClearPromptLine(inputBuffer.Length, promptPrefixLength);

                        Console.Write(inputBuffer.ToString());
                        Console.SetCursorPosition(promptPrefixLength + inputBufferIndex, Console.CursorTop);
                    }
                }
            }
        }

        private void OutputPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            Console.Write(promptPrefix);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void OutputResponse()
        {
            string response = outputBuffer.ToString();
            if (response.Length > 0)
            {
                ConsoleColor stockColor = Console.BackgroundColor;
                if (outputAttention)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.Write(response);
                if (outputAttention)
                {
                    Console.BackgroundColor = stockColor;
                }
            }
        }

        private bool ProcessUserInput()
        {
            outputBuffer.Length = 0;
            outputAttention = false;
            if (!interruptedByCtrlCombination)
            {
                return ProcessInput();
            }
            else
            {
                return ProcessCtrlCombinations();
            }
        }

        private bool ProcessInput()
        {
            if (attemptingToExit)
            {
                attemptingToExit = false;
            }
            if (activeCommand == Command.None)
            {
                ParseCommandAndArguments();
            }
            switch (activeCommand)
            {
                case Command.None:
                    {
                        outputBuffer.Append(USAGE_TEXT);
                    }
                    break;
                case Command.Create:
                    {
                        if (ProcessCreateCommand())
                        {
                            activeCommand = Command.None;
                        }
                    }
                    break;
                case Command.List:
                    {
                        if (ProcessListCommand())
                        {
                            activeCommand = Command.None;
                        }
                       
                    }
                    break;
            }
            UpdatePromptPrefix();
            return false;
        }

        private bool ProcessCtrlCombinations()
        {
            if ((keyPressedWithCtrl == ConsoleKey.Q))
            {
                if (attemptingToExit)
                {
                    return true;
                }
                outputBuffer.Append("Exit? Press CTRL+Q again to exit\n");
                outputAttention = true;
                attemptingToExit = true;

            }
            else if (attemptingToExit)
            {
                attemptingToExit = false;
            }

            return false;
        }

        private void ParseCommandAndArguments()
        {
            string[] args = inputBuffer.ToString().Split(' ', '\t');
            if (args.Length > 0)
            {
                if (commandMap.ContainsKey(args[0]))
                {
                    activeCommand = commandMap[args[0]];
                    switch (activeCommand)
                    {
                        case Command.None:
                            {
                                return;
                            }
                        case Command.Create:
                            {
                                if (args.Length == 1)
                                {
                                    return;
                                }
                                else if (args.Length == 2)
                                {
                                    if (nameToProductTypeMap.ContainsKey(args[1]))
                                    {
                                        Type productType = nameToProductTypeMap[args[1]];
                                        InitCreateCommand(productType);
                                    }
                                }
                                else if (args.Length > 2)
                                {
                                    Debug.Assert(false);
                                }
                            }
                            break;
                        case Command.List:
                            {
                                if (args.Length == 1)
                                {
                                    return;
                                }
                                else if (args.Length == 2)
                                {
                                    if (nameToProductTypeMap.ContainsKey(args[1]))
                                    {
                                        listProductType = nameToProductTypeMap[args[1]];
                                    }
                                }
                                else if (args.Length > 2)
                                {
                                    Debug.Assert(false);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private bool ProcessCreateCommand()
        {
            if (createInputRequested)
            {
                if (createProductType == null)
                {
                    string arg = inputBuffer.ToString();
                    if (nameToProductTypeMap.ContainsKey(arg))
                    {
                        Type productType = nameToProductTypeMap[arg];
                        InitCreateCommand(productType);

                        OutputProductPropertyValueRequest(createProductType, createPropertyIndex);
                    }
                    else
                    {
                        OutputProductNameRequest();
                    }
                }
                else
                {
                    PropertyInfo info = createProductStuff.PropertyInfoArray[createPropertyIndex];
                    
                    if (createValidator.Validate(createProductType, info, inputBuffer.ToString()))
                    {
                        info.SetValue(createProduct, createValidator.GetLastProperty());

                        if ((createPropertyIndex + 1) < createProductStuff.PropertyInfoArray.Length)
                        {
                            createPropertyIndex++;
                        }
                        else
                        {
                            storage.Add(createProduct);
                            createProductType = null;
                            createProduct = null;
                            createProductStuff = null;
                            createValidator = null;
                            createPropertyIndex = 0;
                            createInputRequested = false;
                            outputBuffer.Append("Product has been successfully created");
                            return true;
                        }
                    }
                    else
                    {
                        outputBuffer.Append(createValidator.GetLastError());
                        outputAttention = true;
                    }

                    OutputProductPropertyValueRequest(createProductType, createPropertyIndex);
                }
            }
            else
            {
                if (createProductType == null)
                {
                    OutputProductNameRequest();
                }
                else
                {
                    OutputProductPropertyValueRequest(createProductType, createPropertyIndex);

                }

                createInputRequested = true;
               
            }
            return false;
        }

        private bool ProcessListCommand()
        {
            if (listProductType == null)
            {
                if (listInputRequested)
                {
                    string arg = inputBuffer.ToString();
                    if (nameToProductTypeMap.ContainsKey(arg))
                    {
                        listProductType = nameToProductTypeMap[arg];
                        MakeProductRecordsListString(storage, productMap, listProductType, outputBuffer);
                        listProductType = null;
                        listInputRequested = false;
                        return true;
                    }
                    else
                    {
                        outputBuffer.Append("Records of all Product types will be listed\n");
                        MakeProductRecordsListString(storage, productMap, listProductType, outputBuffer);
                        listProductType = null;
                        listInputRequested = false;
                        return true;
                    }
                }
                else
                {
                    outputBuffer.Append("You can filter records by Product type name\n");
                    OutputProductNameRequest();
                    listInputRequested = true;
                    return false;
                }
            }
            else
            {
                MakeProductRecordsListString(storage, productMap, listProductType, outputBuffer);
                listProductType = null;
                listInputRequested = false;
                return true;
            }
        }

        private void InitCreateCommand(Type productType)
        {
            createProductType = productType;
            createProduct = (Product)Activator.CreateInstance(createProductType);
            createProductStuff = productMap[createProductType];
            createValidator = validatorMap[createProductType];
            createPropertyIndex = 0;
        }
        private void OutputProductNameRequest()
        {
            outputBuffer.Append("List of product type names:\n");
            foreach (var pair in nameToProductTypeMap)
            {
                outputBuffer.Append("- ");
                outputBuffer.Append(pair.Key);
                outputBuffer.Append("\n");
            }
            outputBuffer.Append("Enter product type name:");
        }
        private void OutputProductPropertyValueRequest(Type productType, int index)
        {
            string name = productMap[productType].ProperPropertyNames[index]; 
            outputBuffer.Append(string.Format("Enter Value for \"{0}\"\n", name));
        }
        private void UpdatePromptPrefix()
        {
            if (activeCommand != Command.None)
            {
                promptPrefix = activeCommand.ToString() + ">";
            }
            else
            {
                promptPrefix = ">";
            }
        }
       
        private void ClearPromptLine(int lineLength, int prefixOffset)
        {
            Console.SetCursorPosition(prefixOffset, Console.CursorTop);
            for (int i = 0; i < lineLength; i++)
                Console.Write(" ");
            Console.SetCursorPosition(prefixOffset, Console.CursorTop);
        }
        private static ProductCachedStuff CacheProductStuff(Type productType)
        {
            var propertyInfoArray = productType.GetProperties();
           
            string[] properPropertyNames = new string[propertyInfoArray.Length];
            for (int i = 0; i < propertyInfoArray.Length; i++)
            {
                PropertyInfo info = propertyInfoArray[i];
                properPropertyNames[i] = ReflectionMisc.GetPropertyName(info);
            }
         
            ProductCachedStuff result = new ProductCachedStuff();
            result.ProductName = ReflectionMisc.GetTypeName(productType);
            result.PropertyInfoArray = propertyInfoArray;
            result.ProperPropertyNames = properPropertyNames;
            return result;
        }

        private static Dictionary<Type, ProductCachedStuff>
            CreateDictionaryOfCachedProductStuff(Type[] productTypes)
        {
            Dictionary<Type, ProductCachedStuff> result = new Dictionary<Type, ProductCachedStuff>();
            foreach (Type type in productTypes)
            {
                result.Add(type, CacheProductStuff(type));
            }
            return result;
        }

        private static void MakeProductPropertyString(Product product, PropertyInfo[] infoArray, string[] properNames, int index, StringBuilder buffer)
        {
            PropertyInfo info = infoArray[index];
            string name = properNames[index];
            string value = info.GetValue(product).ToString();
            buffer.Append(string.Format("Name: \"{0}\" Value: {1}\n", name, value));
        }
        private static void MakeProductPropertyPromptString(PropertyInfo[] infoArray, int index, StringBuilder buffer)
        {
            PropertyInfo info = infoArray[index];
            string name = ReflectionMisc.GetPropertyName(info);
            buffer.Append(string.Format("Enter Value for \"{0}\"\n", name));
        }
        private static void MakeProductRecordsListString(List<Product> products, Dictionary<Type, ProductCachedStuff> productCachedStuffDict, Type filterProduct, StringBuilder buffer)
        {
            foreach (var product in products)
            {
                Type type = product.GetType();
                if (filterProduct != null)
                {
                    if (type.Equals(filterProduct))
                    {
                        Debug.Assert(productCachedStuffDict.ContainsKey(type));
                        ProductCachedStuff cache = productCachedStuffDict[type];
                        buffer.Append(String.Format("Product name: \"{0}\"\n", cache.ProductName));
                        for (int i = 0; i < cache.PropertyInfoArray.Length; i++)
                        {
                            MakeProductPropertyString(product, cache.PropertyInfoArray, cache.ProperPropertyNames, i, buffer);
                        }
                    }
                }
                else
                {
                    Debug.Assert(productCachedStuffDict.ContainsKey(type));
                    ProductCachedStuff cache = productCachedStuffDict[type];
                    buffer.Append(String.Format("Product name: \"{0}\"\n", cache.ProductName));
                    for (int i = 0; i < cache.PropertyInfoArray.Length; i++)
                    {
                        MakeProductPropertyString(product, cache.PropertyInfoArray, cache.ProperPropertyNames, i, buffer);
                    }
                }
            }
        }

#if false
        private static bool TrySetPropertyOnProduct(Product product, int index, string value,
            Dictionary<Type, ProductCachedStuff> productCachedStuffDict, StringBuilder buffer)
        {
            Type type = product.GetType();
            Debug.Assert(productCachedStuffDict.ContainsKey(type));
            ProductCachedStuff cache = productCachedStuffDict[type];
            ProductValidator validator = productCachedStuffDict[type].Validator;
            if (validator.Validate(type, cache.PropertyInfoArray[index], value))
            {
                cache.PropertyInfoArray[index].SetValue(product, validator.GetLastProperty());
                return true;
            }
            else
            {
                buffer.Append(validator.GetLastError());
                return false;
            }
        }
#endif

    }
}