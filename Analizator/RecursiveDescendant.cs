﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Analizator {
    class RecursiveDescendant {
        /*
         * input w=[0,20,1]
            s->T20T
            T->0|1

            tabela de codificare

            0-identificator
            1-constanta
            20- + (operator)
            s
         */

        enum Stare {
            normala, //q -terminare
            revenire, // r -revenire
            terminare, //t -accept
            eroare  // e -eroare
        }

        List<int> sir_prod;

        public RecursiveDescendant() {

            Analyze(ReadGrammar() , GetInput());
        }

        private List<string> GetInput() {
            var line = File.ReadLines("../../input.txt");
            List<string> w = line.First().Split(' ').ToList();
            w.Insert(0 , "null");
            return w;
        }
        List<string> NonTerminals;
        List<string> Terminals;
        String startSymbol;
        private Dictionary<string , List<Production>> ReadGrammar() {
            int i = 0;
            var logFile = File.ReadLines("../../gramatica.txt");
            var dict = new Dictionary<string , List<Production>>();
            foreach (var x in logFile) {
                if (i == 0) {
                    Terminals = new List<string>(x.Split(','));
                } else {
                    if (i == 1) {
                        NonTerminals = new List<string>(x.Split(','));
                    } else {
                        if (i == 2) {
                            startSymbol = x;
                        } else {
                            if (i > 2) {
                                var strings = x.Split(new string[] { "->" } , StringSplitOptions.None);
                                if (dict.ContainsKey(strings[0]))
                                    dict[strings[0]].Add(new Production(strings[1]));
                                else {
                                    var list = new List<Production> {
                                        new Production(strings[1])
                                    };
                                    dict.Add(strings[0] , list);
                                }
                            }
                        }
                    }
                }
                i++;
            }
            return dict;
        }

        private void Analyze(Dictionary<string , List<Production>> productii , List<string> input) {


            var stare = Stare.normala;
            int i = 1;
            int n = input.Count;

            var alfa = new Stack<string>();
            var beta = new Stack<string>();//trebuie sa contina simbolul de start
            beta.Push(startSymbol);
            var currentNonT = startSymbol;
            int j = 0;
            while (stare != Stare.terminare && stare != Stare.eroare) {
                if (stare == Stare.normala) {
                    if (beta.Count() == 0 && i == n)
                        stare = Stare.terminare;
                    else {
                        if (NonTerminals.IndexOf(beta.Peek()) >= 0 || beta.Peek() == startSymbol) {//e nonterminal sau simbol start
                            alfa.Push(beta.Pop() + (j + 1));
                            var rez = alfa.Peek();
                            
                            foreach (var x in productii[GetNonTerminal(rez)][GetProductionNumber(rez) - 1].Productii) {
                                beta.Push(x);
                            }

                        } else {
                            if (i < n && beta.Peek() == input[i]) {
                                i = i + 1;
                                var rez = beta.Pop();
                                var x = rez.Substring(0 , 1);
                                j = 0;
                                alfa.Push(x);
                            } else
                                stare = Stare.revenire;
                        }
                    }
                } else {
                    if (stare == Stare.revenire) {
                        if (Terminals.IndexOf(alfa.Peek()) >= 0) {//4 revenire daca e terminal
                            i = i - 1;
                            var rez = alfa.Pop();
                            beta.Push(rez);
                        } else {
                            if (GetProductionNumber(alfa.Peek()) < productii[currentNonT].Count()) {
                                stare = Stare.normala;
                                var rez = alfa.Pop();

                                var nr = GetProductionNumber(rez);
                                rez = GetNonTerminal(rez);

                                for (var k = 0 ; k < productii[rez][nr - 1].Productii.Count ; k++) {
                                    beta.Pop();
                                }

                                foreach (var x in productii[rez][nr].Productii) {
                                    beta.Push(x);
                                }
                                alfa.Push(rez + ++nr);
                                j++;
                            } else {
                                if (i == 1 && currentNonT == startSymbol) {
                                    stare = Stare.eroare;
                                } else {
                                    var rez = alfa.Pop();
                                    beta.Pop();
                                    beta.Push(GetNonTerminal(rez));

                                }
                            }
                        }
                    }
                }
            }
            if (stare == Stare.eroare) {
                Console.WriteLine("eroare");
            } else {
                Console.WriteLine("Secventa e acceptata");
                //construireSirProd(alfa);
            }

        }


        private int GetProductionNumber(string rez) {
            return int.Parse(Regex.Match(rez , @"\d+$").Value);
        }

        private string GetNonTerminal(string rez) {
            string pattern = @"\d+$";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            rez = rgx.Replace(rez , replacement);
            return rez;
        }

        private void construireSirProd(Stack<string> alfa) {
            string sir = "";
            while (alfa.Count > 0) {
                if (int.Parse(alfa.Peek()) > 0) {
                    sir = alfa.Peek() + ' ' + sir;
                }
                alfa.Pop();
            }
        }
    }
}