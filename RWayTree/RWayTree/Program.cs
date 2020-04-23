/* Name: R-Way Tree Data Structure (Assignment #2)
 * Author: Brian Patrick & Brianna Drew
 * Date Created: February 10th, 2020
 * Last Modified: March 8th, 2020
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RWayTree
{

    public static class MyGlobals
    {
        public static bool exit_bool, error_bool;
    }

    public interface IContainer<T>
    {
        void MakeEmpty();
        bool Empty();
        int Size();
    }

    //-------------------------------------------------------------------------

    public interface ITrie<T> : IContainer<T>
    {
        bool Insert(string key, T value);
        bool Remove(string key);
        T Value(string key);
    }

    //-------------------------------------------------------------------------

    class Trie<T> : ITrie<T>
    {
        private Node root;          // Root node of the Trie

        class Node
        {
            public T value;         // Value at Node; otherwise default
            public int numValues;   // Number of descendent values of a Node 
            public Node[] child;    // Branching for each letter 'a' .. 'z'

            // Node
            // Creates an empty Node
            // All children are set to null by default
            // Time complexity:  O(1)

            public Node()
            {
                value = default(T);
                numValues = 0;
                child = new Node[26];
            }
        }

        // Trie
        // Creates an empty Trie
        // Time complexity:  O(1)

        public Trie()
        {
            MakeEmpty();
        }

        // Public Insert
        // Calls the private Insert which carries out the actual insertion
        // Returns true if successful; false otherwise

        public bool Insert(string key, T value)
        {
            return Insert(root, key, 0, value);
        }

        // Private Insert
        // Inserts the key/value pair into the Trie
        // Returns true if the insertion was successful; false otherwise
        // Note: Duplicate keys are ignored
        // Time complexity:  O(L) where L is the length of the key

        private bool Insert(Node p, string key, int j, T value)
        {
            int i;

            if (j == key.Length)
            {
                if (p.value.Equals(default(T)))
                {
                    // Sets the value at the Node
                    p.value = value;
                    p.numValues++;
                    return true;
                }
                // Duplicate keys are ignored (unsuccessful insertion)
                else
                    return false;
            }
            else
            {
                // Maps a character to an index
                i = Char.ToLower(key[j]) - 'a';

                // Creates a new Node if the link is null
                // Note: Node is initialized to the default value
                if (p.child[i] == null)
                    p.child[i] = new Node();

                // If the inseration is successful
                if (Insert(p.child[i], key, j + 1, value))
                {
                    // Increase number of descendent values by one
                    p.numValues++;
                    return true;
                }
                else
                    return false;
            }
        }

        // Value
        // Returns the value associated with a key; otherwise default
        // Time complexity:  O(L) where L is the length of the key

        public T Value(string key)
        {
            int i;
            Node p = root;

            // Traverses the links character by character
            foreach (char ch in key)
            {
                i = Char.ToLower(ch) - 'a';
                if (p.child[i] == null)
                {
                    Console.WriteLine("ERROR: Key does not exist.\n");
                    return default(T);    // Key is too long
                }
                else
                    p = p.child[i];
            }
            Console.WriteLine("The value associated with " + key + " is " + p.value + ".\n");
            return p.value;               // Returns the value or default
        }

        // Public Remove
        // Calls the private Remove that carries out the actual deletion
        // Returns true if successful; false otherwise

        public bool Remove(string key)
        {
            return Remove(root, key, 0);
        }

        // Private Remove
        // Removes the value associated with the given key
        // Time complexity:  O(L) where L is the length of the key

        private bool Remove(Node p, string key, int j)
        {
            int i;

            // Key not found
            if (p == null)
                return false;

            else if (j == key.Length)
            {
                // Key/value pair found
                if (!p.value.Equals(default(T)))
                {
                    p.value = default(T);
                    p.numValues--;
                    return true;
                }
                // No value with associated key
                else
                    return false;
            }

            else
            {
                i = Char.ToLower(key[j]) - 'a';

                // If the deletion is successful
                if (Remove(p.child[i], key, j + 1))
                {
                    // Decrease number of descendent values by one and
                    // Remove Nodes with no remaining descendents
                    if (p.child[i].numValues == 0)
                        p.child[i] = null;
                    p.numValues--;
                    return true;
                }
                else
                    return false;
            }
        }

        // Partial Match
        // Returns list of keys that match a provided pattern

        public List<string> PartialMatch(string pattern)
        {
            List<string> partial_list = new List<string>();
            Regex patt = new Regex(pattern);        // Convert passed string to a regular expression pattern
            return PartialMatch(root, patt, "", partial_list);
        }

        private List<string> PartialMatch(Node p, Regex pattern, string key, List<string> partial_list)
        {
            int i;
            bool match;
            if (p != null)      // if the current node is not null...
            {
                match = pattern.IsMatch(key);       // determine whether or not the key of the current node matches the pattern
                if (!p.value.Equals(default(T)) && match)       // if the key of the current node matches the pattern...
                {
                    partial_list.Add(key);      // add the key of the current node to the list
                }
                for (i = 0; i < 26; i++)        // traverse the r-way tree
                {
                    PartialMatch(p.child[i], pattern, key + (char)(i + 'a'), partial_list);
                }
                return partial_list;
            }
            else        // if the current node is null...
            {
                return partial_list;        // return the list of keys
            }
        }

        // Autocomplete
        // Returns a list of keys that begin with a provided prefix

        public List<string> Autocomplete(string prefix)
        {
            List<string> auto_list = new List<string>();
            return Autocomplete(root, prefix, "", auto_list);
        }

        private List<string> Autocomplete(Node p, string prefix, string key, List<string> auto_list)
        {
            int i;
            bool compare;
            if (p != null)      // if the current node is not null...
            {
                compare = StringCompare(prefix, key);       // determine whether the key has the prefix
                if (!p.value.Equals(default(T)) && compare)     // if the key has the prefix...
                {
                    auto_list.Add(key);     // add the key to the list
                }
                for (i = 0; i < 26; i++)        // traverse the r-way tree
                {
                    Autocomplete(p.child[i], prefix, key + (char)(i + 'a'), auto_list);
                }
                return auto_list;       // return list of keys
            }
            else { return auto_list; }
        }

        // StringCompare
        // Compares each character of two string to see if they are the same (true if match, false otherwise)
        private bool StringCompare(string prefix, string key)
        {
            if (key.Length < prefix.Length)     // if the length of the key is less than the length of the prefix, we know the key cannot have the prefix
            {
                return false;
            }
            for (int i = 0; i < prefix.Length; i++)     // compare each character of the prefix to the first characters of the key
            {
                if (Char.ToLower(prefix[i]) != key[i])
                {
                    return false;       // not a matching prefix
                }
            }
            return true;        // matching prefix
        }

        // MakeEmpty
        // Creates an empty Trie
        // Time complexity:  O(1)

        public void MakeEmpty()
        {
            root = new Node();
        }

        // Empty
        // Returns true if the Trie is empty; false otherwise
        // Time complexity:  O(1)

        public bool Empty()
        {
            return root.numValues == 0;
        }

        // Size
        // Returns the number of Trie values
        // Time complexity:  O(1)

        public int Size()
        {
            return root.numValues;
        }

        // Public Print
        // Calls private Print to carry out the actual printing

        public void Print()
        {
            Print(root, "");
        }

        // Private Print
        // Outputs the key/value pairs ordered by keys
        // Time complexity:  O(S) where S is the total length of the keys

        private void Print(Node p, string key)
        {
            int i;

            if (p != null)
            {
                //if (!p.value.Equals(default(T)))
                Console.WriteLine(key + " " + p.value + " " + p.numValues);
                for (i = 0; i < 26; i++)
                    Print(p.child[i], key + (char)(i + 'a'));
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string choice, key, str_value;
            bool conversion, success;
            int intchoice, value;

            Trie<int> T;
            T = new Trie<int>();

            while (!MyGlobals.exit_bool)
            {
                MyGlobals.error_bool = true;
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*                    R-WAY TREES                     *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*    1 = CREATE A NEW R-WAY TREE                     *");
                Console.WriteLine("*    2 = INSERT KEY/VALUE                            *");
                Console.WriteLine("*    3 = DELETE KEY/VALUE                            *");
                Console.WriteLine("*    4 = FIND VALUE                                  *");
                Console.WriteLine("*    5 = AUTOCOMPLETE FIND KEYS                      *");
                Console.WriteLine("*    6 = FIND PARTIAL KEY MATCHES                    *");
                Console.WriteLine("*    7 = GET SIZE OF R-WAY TREE                      *");
                Console.WriteLine("*    8 = PRINT R-WAY TREE                            *");
                Console.WriteLine("*    9 = EXIT PROGRAM                                *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************\n");

                do
                {
                    choice = Console.ReadLine();        // get user choice
                    conversion = Int32.TryParse(choice, out intchoice);
                    if (!conversion)        // if the user's entry is not an integer
                    {
                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");
                    }
                    else
                    {
                        MyGlobals.error_bool = false;
                        break;
                    }
                    Console.WriteLine();
                } while (MyGlobals.error_bool);

                MyGlobals.error_bool = true;
                Console.WriteLine();

                switch (intchoice)
                {
                    case 1:     // Create new R-Way tree
                        T.MakeEmpty();
                        Console.WriteLine("New R-Way Tree created successfully.\n");
                        break;
                    case 2:     // Insert new key/value pair
                        Console.WriteLine("What key would you like to insert?");
                        key = Console.ReadLine();
                        Console.WriteLine("What value would you like to insert?");
                        do
                        {
                            str_value = Console.ReadLine();
                            conversion = Int32.TryParse(str_value, out value);
                            if (!conversion)
                            {
                                Console.WriteLine("ERROR: Invalid option. Please try again.\n");
                            }
                            else
                            {
                                MyGlobals.error_bool = false;
                                break;
                            }
                        } while (MyGlobals.error_bool);
                        MyGlobals.error_bool = true;
                        Console.WriteLine();
                        success = T.Insert(key, value);
                        if (success)
                        {
                            Console.WriteLine("Successfully inserted " + key + " & " + value + " into the R-Way tree.\n");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Insertion unsuccessful (key/value pair already exists).\n");
                        }
                        break;
                    case 3:     // Delete an existing key/value pair
                        Console.WriteLine("What key would you like to delete?");
                        key = Console.ReadLine();
                        success = T.Remove(key);
                        if (success)
                        {
                            Console.WriteLine("Successfully removed " + key + " from the R-Way tree.\n");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: key/value pair not found.\n");
                        }
                        break;
                    case 4:     // Find the value associated with a key
                        Console.WriteLine("Which key would you like to find?");
                        key = Console.ReadLine();
                        _ = T.Value(key);
                        Console.WriteLine("\n");
                        break;
                    case 5:     // Find keys that match a prefix
                        Console.WriteLine("What is the prefix you are looking for in keys?");
                        key = Console.ReadLine();
                        List<string> Autocompletes = T.Autocomplete(key);
                        Console.WriteLine("Keys that match prefix: ");
                        foreach (string element in Autocompletes)
                        {
                            Console.Write(element + " ");
                        }
                        Console.WriteLine("\n");
                        break;
                    case 6:     // Find keys that match a pattern
                        Console.WriteLine("What is the pattern you are looking for in keys?");
                        key = Console.ReadLine();
                        List<string> PartialMatches = T.PartialMatch(key);
                        Console.WriteLine("Keys that match pattern: ");
                        foreach (string element in PartialMatches)
                        {
                            Console.Write(element + " ");
                        }
                        Console.WriteLine("\n");
                        break;
                    case 7:     // Get the size of the current R-Way Tree
                        Console.WriteLine("Size of R-Way Tree: " + T.Size() + ".\n");
                        break;
                    case 8:     // Print R-Way tree
                        T.Print();
                        break;
                    case 9:     // Exit program
                        MyGlobals.exit_bool = true;
                        Console.WriteLine("Exiting program...");
                        Environment.Exit(0);
                        break;
                    default:        // Invalid integer menu choice
                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");
                        break;
                }
            }
        }
    }
}
