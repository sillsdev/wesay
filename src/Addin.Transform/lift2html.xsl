<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html"  omit-xml-declaration="yes"/>
  <xsl:template match="*"/>

  <xsl:template match="text()">
	<xsl:if test="not(normalize-space() = '')">
	  <xsl:value-of select="."/>
	</xsl:if>
  </xsl:template>

  <xsl:template match="/">
	<xsl:text>Sorry, this doesn't actually do anything yet.</xsl:text>
	<xsl:apply-templates select="//entry"/>
  </xsl:template>

  <xsl:template  match="entry">
	<p>
	<xsl:apply-templates />
	</p>
  </xsl:template>

  <xsl:template match="lexical-unit">
	<b>
	  <xsl:apply-templates select ="form" />
	</b>
  </xsl:template>


  <xsl:template match="text">
	<xsl:value-of select="normalize-space()"/>
  </xsl:template>

</xsl:stylesheet>
