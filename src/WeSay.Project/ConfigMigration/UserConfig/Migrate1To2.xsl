<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- don't do anything to other versions -->
  <xsl:template match="configuration[@version='1']">
	<configuration version="2">
	  <xsl:apply-templates mode ="Migrate1To2"/>
	</configuration>
  </xsl:template>

  <xsl:template match="@*|node()" mode="Migrate1To2">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate1To2"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()" mode="Migrate1To2">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate1To2"/>
	</xsl:copy>
  </xsl:template>

  <!-- ============= actual changing starts here ================ -->
  <xsl:template match="language" mode="Migrate1To2">
	<language><xsl:value-of select="substring-after(text(), 'wesay-')" /></language>
  </xsl:template>

</xsl:stylesheet>