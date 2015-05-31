ifdef SYSTEMROOT
	CSC=csc.exe
	FixPath = $(subst /,\\,$1)
else
	CSC=mcs
	RM=rm -f
	FixPath = $1
endif

LLParserGen.exe : FirstFollow.cs LexParserHelper.cs Main.cs regexp.cs
	$(CSC) -out:LLParserGen.exe *.cs \
		$(call FixPath,$(wildcard ../LLParserGenLib/*.cs)) \
		$(call FixPath,$(wildcard ./U/*.cs))
	# cp LLParserGen.exe ~/bin/.

clean :
	$(RM) -f LLParserGen.exe M.cs
