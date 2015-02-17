
Test.exe : M.cs main.cs
	mcs -out:./Test.exe M.cs main.cs ../LLParserGenLib/*.cs

M.cs : M.gram
	~/bin/LLParserGen.exe M.gram

clean:
	-rm -f Test.exe M.cs
