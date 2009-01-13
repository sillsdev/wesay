<?xml version="1.0"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:template match="/">
	<optionsList id="DDP4" guid="{//CmPossibilityList/@id}">
	  <options>
		<xsl:apply-templates select="//CmSemanticDomain"/>
	  </options>
	</optionsList>
  </xsl:template>

  <xsl:template match="CmSemanticDomain">
	<xsl:variable name="id">
	  <xsl:value-of select="Abbreviation7/AUni[@ws='en']"/>
	  <xsl:text> </xsl:text>
	  <xsl:value-of select="Name7/AUni[@ws='en']"/>
	</xsl:variable>
	<option>
	  <key>
		<xsl:value-of select="$id"/>
	  </key>
	  <name>
		<xsl:apply-templates select="Name7/AUni"/>
	  </name>
	  <abbreviation>
		<xsl:apply-templates select="Abbreviation7/AUni"/>
	  </abbreviation>
	  <description>
		<xsl:apply-templates select="Description7/AStr"/>
	  </description>
	  <searchKeys>
		<xsl:apply-templates select="Questions66/CmDomainQ/ExampleWords67/AUni"/>
	  </searchKeys>
	</option>
  </xsl:template>

  <xsl:template match="AUni|AStr">
	<xsl:variable name="content">
	  <xsl:value-of select="normalize-space(.)"/>
	</xsl:variable>

	<xsl:if test="$content != ''">
	  <form ws="{@ws}">
		<xsl:value-of select="$content"/>
	  </form>
	</xsl:if>
  </xsl:template>

</xsl:transform>