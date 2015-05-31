ifdef SYSTEMROOT
	CSC=csc.exe
	FixPath = $(subst /,\\,$1)
else
	CSC=mcs
	RM=rm -f
	FixPath = $1
endif

LLParserGenLib.dll : 
	$(CSC) -target:library -out:LLParserGenLib.dll *.cs

clean:
	$(RM) -f LLParserGenLib.dll
