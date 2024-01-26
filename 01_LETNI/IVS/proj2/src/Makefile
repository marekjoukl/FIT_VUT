# Name for the zip file, which will be submitted.
TARGET = xfindr01_xjoukl00_xkozan01_xolesa00
# Name od the binary for computing the standart deviation.
STDDEV = stddev
# Name of the binary of the calculator app.
CALCULATOR = calculator
# Name of the repository.
DIR = $(notdir $(abspath $(CURDIR)/..))

# Compiler settings
CXX = g++
CXXFLAGS = -g

.PHONY: all pack clean test doc run profile build buildFolder dependencies

all: dependencies build profile

# Install all necessary neccesary dependencies.
dependencies:
	@which build-essential > /dev/null || (sudo apt-get -qq -y update && sudo apt install -qq -y build-essential)
	@which cmake > /dev/null || (sudo apt install -qq -y cmake)
	@which qtbase5-dev > /dev/null || (sudo apt install -qq -y qtbase5-dev)
	@which doxygen > /dev/null || (sudo apt install -qq -y doxygen)

# Compile the binary for standart deviation.
profile: dependencies buildFolder
	@cd build && $(CXX) $(CXXFLAGS) ../standardDeviation.cpp -o $(STDDEV)

# Run test for the standart deviation.
test:
	@cd tests && make -s && ./mathTests.out
	
# Create folder which will hold all the compiled files.
buildFolder:
	@mkdir -p build

# Compile the calculator binary.
build: dependencies buildFolder
	@cd build && cmake .. && make -s

# Clean the repository to be ready to be submitted.
clean:
	@rm -rf build
	@rm -rf doc
	@cd tests && make -s clean

# Pack the directory for submission.
pack: clean $(TARGET) 

# Generate the program documentation.
doc: dependencies
	@doxygen Doxyfile

# Prepare and zip folder for submission.
$(TARGET):
	@cd .. && cd .. && mkdir -p $(TARGET) > /dev/null
	@cd .. && cd ../$(TARGET) && mkdir -p doc install > /dev/null
	@cd .. && cd .. && cp -a $(DIR) ./$(TARGET) > /dev/null && mv ./$(TARGET)/$(DIR) ./$(TARGET)/repo
	@$(MAKE) doc
	@cd .. && cd .. && cp -r $(DIR)/src/doc/* ./$(TARGET)/doc > /dev/null
	@cd .. && cd .. && cp -r $(DIR)/src/install/*.deb ./$(TARGET)/install > /dev/null
	@cd .. && cd .. && zip -r $(TARGET).zip $(TARGET) > /dev/null
	@cd .. && cd .. && rm -rf $(TARGET) > /dev/null

# Run the calculator binary.
run: build
	./build/$(CALCULATOR)
