all:
	make -C LLParserGen
	make -C LLParserGenLib
	make -C LLParserGenTest

clean:
	make -C LLParserGen     clean
	make -C LLParserGenLib  clean
	make -C LLParserGenTest clean
