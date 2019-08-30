#!/bin/bash
/c/Program\ Files/dotnet/dotnet ../LLParserGen/bin/Debug/netcoreapp2.1/LLParserGen.dll DateExprParser.gram
cp DateExpr.cs DateExprParser.cs DateExprParser.gram ../../et_dw/ET_DW_Builder/DateParser/.
cp ../LLParserGenLib/U_Lexer.cs ../LLParserGenLib/U_LLParserLexerLib.cs ../../et_dw/ET_DW_Builder/DateParser/.
