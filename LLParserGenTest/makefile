ifdef SYSTEMROOT
	CSC=csc.exe
	FixPath = $(subst /,\\,$1)
else
	CSC=mcs
	RM=rm -f
	FixPath = $1
endif

Test.exe : Ass.cs Context.cs Graph.cs main.cs M.cs U.cs
	$(CSC) -debug+ -out:$(call FixPath,./Test.exe) $(call FixPath,$(wildcard ../LLParserGenLib/*.cs)) \
		Ass.cs Context.cs Graph.cs main.cs M.cs U.cs

M.cs : M.gram
	../LLParserGen/LLParserGen.exe M.gram

clean:
	$(RM) Test.exe M.cs
