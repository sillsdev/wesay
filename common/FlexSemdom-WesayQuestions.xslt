<?xml version="1.0"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:param name="lang" select="'en'"/>

  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:template match="/">
	<semantic-domain-questions semantic-domain-type="DDP4" lang="{$lang}">
	  <xsl:apply-templates select="//CmSemanticDomain"/>
	</semantic-domain-questions>
  </xsl:template>

  <xsl:template match="CmSemanticDomain">
	<xsl:variable name="id">
	  <xsl:value-of select="Abbreviation7/AUni[@ws='en']"/>
	  <xsl:text> </xsl:text>
	  <xsl:value-of select="Name7/AUni[@ws='en']"/>
	</xsl:variable>

	<semantic-domain guid="{@id}" id="{$id}">
	  <xsl:apply-templates select="Questions66/CmDomainQuestion"/>
	</semantic-domain>
  </xsl:template>

  <xsl:template match="CmDomainQuestion">
	<xsl:variable name="question">
	  <xsl:value-of select="normalize-space(Question67/AUni[@ws=$lang])"/>
	</xsl:variable>

	<xsl:variable name="questionWithoutNumber">
	  <xsl:choose>
		<xsl:when test="starts-with($question, '(')">
		  <xsl:value-of select="substring-after($question, ') ')"/>
		</xsl:when>
		<xsl:otherwise>
		  <xsl:value-of select="$question"/>
		</xsl:otherwise>
	  </xsl:choose>
	</xsl:variable>

	<xsl:if test="$question != ''">
	  <question>
		<xsl:value-of select="$questionWithoutNumber"/>
	  </question>
	</xsl:if>
  </xsl:template>

</xsl:transform>