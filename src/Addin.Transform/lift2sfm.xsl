<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [
<!ENTITY nl "
">
]>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="text" omit-xml-declaration="yes"/>
   <xsl:template match="*"/>

   <xsl:template match="text()">
	   <xsl:if test="not(normalize-space() = '')">
		   <xsl:value-of select="."/>
	   </xsl:if>
   </xsl:template>

	<xsl:template match="/">
		<xsl:apply-templates select="//entry"/>
	</xsl:template>

	<xsl:template  match="entry">

		<xsl:apply-templates />
		<xsl:if test="@dateModified">
			<xsl:text>&nl;\dt </xsl:text><xsl:value-of select="substring(@dateModified, 1, 10)"/>
		</xsl:if>
		<xsl:text>&nl;</xsl:text>
	</xsl:template>

	<xsl:template match="lexical-unit">
		<xsl:apply-templates>
			<xsl:with-param name="prefix">lx</xsl:with-param>
		</xsl:apply-templates>
	</xsl:template>


	  <xsl:template match="field">
		<xsl:apply-templates select="form">
			<xsl:with-param name="prefix">
				<xsl:value-of select="@tag"/>
			 </xsl:with-param>
		</xsl:apply-templates>
	</xsl:template>


	<xsl:template match="trait">
		<xsl:text>&nl;\</xsl:text><xsl:value-of select="@name"/>
	  <xsl:text>   </xsl:text>
		<xsl:value-of select="@value"/>
	</xsl:template>

	 <xsl:template match="sense">
		 <xsl:text>&nl;\ps </xsl:text><xsl:value-of select="grammatical-info/@value"/>
		<xsl:apply-templates select="gloss">
			<xsl:sort select="@lang"/>
		</xsl:apply-templates>
		 <xsl:apply-templates select="trait"/>
		 <xsl:apply-templates select="field"/>
		 <xsl:apply-templates select="example"/>
		 <xsl:apply-templates select="note"/>
	 </xsl:template>

<xsl:template match="example">
	<xsl:apply-templates select="form">
		<xsl:with-param name="prefix">x</xsl:with-param>
	</xsl:apply-templates>
	<xsl:apply-templates select="translation"/>
</xsl:template>


	<xsl:template match="note">
		<xsl:apply-templates select="form">
			<xsl:with-param name="prefix">nt</xsl:with-param>
		</xsl:apply-templates>
	</xsl:template>


	<xsl:template match="translation">
		<xsl:apply-templates select="form">
			<xsl:with-param name="prefix">x</xsl:with-param>
		</xsl:apply-templates>
	 </xsl:template>


	<xsl:template match="gloss">
		<xsl:choose>
			<xsl:when test="not(preceding-sibling::gloss[@lang=current()/@lang])"><!-- are we the first gloss with this lang -->
				<xsl:text>&#13;\g_</xsl:text><xsl:value-of select="@lang"/><xsl:text> </xsl:text>
			</xsl:when>
			<xsl:otherwise>; </xsl:otherwise>
		</xsl:choose>
		<xsl:apply-templates select="text"/>
	</xsl:template>

	  <xsl:template match="form">
		  <xsl:param name="prefix"/>

		  <xsl:text>&nl;\</xsl:text><xsl:value-of select="$prefix"/><xsl:text>_</xsl:text>
		<xsl:value-of select="@lang"/><xsl:text>   </xsl:text>
		<xsl:apply-templates select="text" />
	  </xsl:template>

	<xsl:template match="text">
		<xsl:value-of select="normalize-space()"/>
	</xsl:template>

</xsl:stylesheet>
