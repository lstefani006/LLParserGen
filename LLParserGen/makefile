
LLParserGen.exe : FirstFollow.cs LexParserHelper.cs Main.cs regexp.cs
	mcs -out:LLParserGen.exe *.cs ../LLParserGenLib/*.cs U/*.cs
	cp LLParserGen.exe ~/bin/.

clean :
	-rm -f LLParserGen.exe
