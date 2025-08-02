#!/bin/bash

# Questionnaire Module Extraction Script
# This script helps extract the questionnaire module to other Angular projects

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}Questionnaire Module Extraction Script${NC}"
echo "=============================================="

# Check if target directory is provided
if [ -z "$1" ]; then
    echo -e "${RED}Error: Please provide the target directory${NC}"
    echo "Usage: ./extract.sh <target-directory>"
    echo "Example: ./extract.sh /path/to/your/angular-project/src/app"
    exit 1
fi

TARGET_DIR="$1"
SOURCE_DIR="$(dirname "$0")"

echo -e "${YELLOW}Source directory:${NC} $SOURCE_DIR"
echo -e "${YELLOW}Target directory:${NC} $TARGET_DIR"

# Check if target directory exists
if [ ! -d "$TARGET_DIR" ]; then
    echo -e "${RED}Error: Target directory does not exist${NC}"
    exit 1
fi

# Create questionnaire directory in target
TARGET_QUESTIONNAIRE_DIR="$TARGET_DIR/questionnaire"
mkdir -p "$TARGET_QUESTIONNAIRE_DIR"

echo -e "${GREEN}Copying questionnaire module...${NC}"

# Copy all files and directories
cp -r "$SOURCE_DIR"/* "$TARGET_QUESTIONNAIRE_DIR/"

echo -e "${GREEN}✅ Questionnaire module extracted successfully!${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Import the module in your app.module.ts:"
echo "   import { QuestionnaireModule } from './questionnaire';"
echo ""
echo "2. Add it to your imports:"
echo "   imports: [QuestionnaireModule]"
echo ""
echo "3. Install required dependencies:"
echo "   npm install @angular/material @angular/cdk"
echo ""
echo -e "${GREEN}The questionnaire module is now ready to use!${NC}" 