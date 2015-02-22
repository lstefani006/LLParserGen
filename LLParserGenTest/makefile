
Test.exe : Ass.cs Context.cs Graph.cs main.cs M.cs U.cs
	mcs -out:./Test.exe ../LLParserGenLib/*.cs Ass.cs Context.cs Graph.cs main.cs M.cs U.cs

M.cs : M.gram
	~/bin/LLParserGen.exe M.gram

clean:
	-rm -f Test.exe M.cs
