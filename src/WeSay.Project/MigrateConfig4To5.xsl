<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- don't do anything to other versions -->
  <xsl:template match="configuration[@version='4']">
	<configuration version="5">
	  <xsl:apply-templates mode ="Migrate4To5"/>
	</configuration>
  </xsl:template>

  <xsl:template match="@*|node()" mode="Migrate4To5">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate4To5"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()" mode="identity">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="identity"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="configuration[@version!='4']">
	<configuration>
	  <xsl:attribute name="version">
		<xsl:value-of select="current()/@version"/>
	  </xsl:attribute>
	  <xsl:apply-templates  mode ="identity"></xsl:apply-templates>
	</configuration>
  </xsl:template>


  <!-- ============= Simplify task setup, removing stuff that never
					  really belonged in user setup files -->

  <xsl:template match="task[@id='Dashboard']" mode="Migrate4To5">
	<task taskName="Dashboard">
	  <xsl:attribute name="visible">
		<xsl:value-of select="current()/@visible"/>
	  </xsl:attribute>
	</task>
  </xsl:template>

  <xsl:template match="task[@id='Dictionary']" mode="Migrate4To5">
	<task taskName="Dictionary">
	  <xsl:attribute name="visible">
		<xsl:value-of select="current()/@visible"/>
	  </xsl:attribute>
	</task>
  </xsl:template>

  <xsl:template match="task[@class='WeSay.LexicalTools.MissingInfoTask']" mode="Migrate4To5">
	<task taskName="AddMissingInfo">
	  <xsl:attribute name="visible">
		<xsl:value-of select="current()/@visible"/>
	  </xsl:attribute>
	  <xsl:apply-templates select='label|description|field|showfields|readOnly' mode ="identity"></xsl:apply-templates>
	</task>
  </xsl:template>

  <xsl:template match="task[@class='WeSay.LexicalTools.GatherWordListTask']" mode="Migrate4To5">
	<task taskName="GatherWordList">
	  <xsl:attribute name="visible">
		<xsl:value-of select="current()/@visible"/>
	  </xsl:attribute>
	  <xsl:apply-templates select='wordListWritingSystemId|wordListFileName' mode ="identity"></xsl:apply-templates>
	</task>
  </xsl:template>

  <xsl:template match="task[@id='Gather Words By Domain']" mode="Migrate4To5">
	<task taskName="GatherWordsBySemanticDomains">
	  <xsl:attribute name="visible">
		<xsl:value-of select="current()/@visible"/>
	  </xsl:attribute>
	  <xsl:apply-templates select="semanticDomainsQuestionFileName"  mode="identity"/>
	</task>
  </xsl:template>


</xsl:stylesheet>